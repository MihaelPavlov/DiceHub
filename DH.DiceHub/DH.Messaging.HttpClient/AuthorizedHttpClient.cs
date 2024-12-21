using DH.Messaging.HttpClient.Enums;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Net.Http.Headers;
using System.Net;
using System.Text.Json;
using System.Text;
using DH.Messaging.HttpClient.Helpers;

namespace DH.Messaging.HttpClient;


public class AuthorizedHttpClient : IAuthorizedHttpClient
{
    static readonly JsonSerializerOptions JSON_OPTIONS = new() { PropertyNameCaseInsensitive = true };
    readonly ApplicationApi _application;
    readonly string _httpClientName;
    readonly string _userSubject;
    readonly string _userAccessToken;
    readonly Uri _applicationUri;
    readonly IHttpClientFactory _httpClientFactory;
    readonly ILogger _logger;

    public Uri ApplicationUri => _applicationUri;

    public AuthorizedHttpClient(ApplicationApi application, string httpClientName, string userId, string userAccessToken, Uri applicationUri, IHttpClientFactory httpClientFactory, ILogger logger)
    {
        _application = application;
        _httpClientName = httpClientName;
        _userSubject = userId;
        _userAccessToken = userAccessToken;
        _applicationUri = applicationUri;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<T> SendAsync<T>(HttpMethod method, Uri uri, StringContent? content, bool isImpersonated, CancellationToken cancellationToken)
    {
        string? responseContent = null;

        try
        {
            var httpMessage = new HttpRequestMessage(method, uri);

            if (content != null)
            {
                httpMessage.Content = content;
            }

            var response = await SendAuthorizedAsync(httpMessage, isImpersonated, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var sanitizedContent = Sanitizer.SanitizeCRLF(await response.Content.ReadAsStringAsync(cancellationToken));

                _logger.LogWarning("Request error StatusCode: {StatusCode} RequestUri: {RequestUri} ResponseContent: {Content}", response.StatusCode, uri, sanitizedContent);
                throw new Exception($"Request error StatusCode: {response.StatusCode}");
            }

            responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (typeof(T) == typeof(AuthorizedHttpClientMockResult))
                return default!;

            try
            {
                var result = JsonSerializer.Deserialize<T>(responseContent, JSON_OPTIONS)
                    ?? throw new Exception($"Cannot deserialize into Type: {typeof(T).FullName} ResponseContent: {responseContent}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Request error RequestUri: {RequestUri}", uri);
                throw;
            }
        }
        catch (JsonException ex)
        {
            var sanitizedResponseContent = Sanitizer.SanitizeCRLF(responseContent);

            _logger.LogError(ex, "Cannot deserialize into Type: {Type} ResponseContent: {ResponseContent}", typeof(T).FullName, sanitizedResponseContent);
            throw new Exception("Request error");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RequestUri: {RequestUri}", uri);
            throw new Exception("Request error");
        }
    }

    async Task<HttpResponseMessage> SendAuthorizedAsync(HttpRequestMessage httpMessage, bool isImpersonated, CancellationToken cancellationToken)
    {
        if (isImpersonated)
        {
            httpMessage.Headers.Add("B2Bsub", _userSubject);
            httpMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _userAccessToken);
        }
        else
        {
            httpMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _userAccessToken);
        }

        using var httpClient = _httpClientFactory.CreateClient(_httpClientName);
        {
            var response = await httpClient.SendAsync(httpMessage, cancellationToken);

            if (response.StatusCode != HttpStatusCode.Unauthorized)
                return response;

            _logger.LogWarning("Request error StatusCode: {StatusCode} RequestUri: {RequestUri}, try to get a new token", response.StatusCode, httpMessage.RequestUri);
            //var accessToken2 = await _accessTokensStore.GetNewAccessTokenAsync(_application, cancellationToken);
            var httpMessage2 = CloneHttpRequestMessage(httpMessage);
            httpMessage2.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _userAccessToken); // TODO: CHANGE with accessToken2
            return await httpClient.SendAsync(httpMessage2, cancellationToken);
        }
    }

    static HttpRequestMessage CloneHttpRequestMessage(HttpRequestMessage original)
    {
        var result = new HttpRequestMessage(original.Method, original.RequestUri);

        if (original.Content != null)
            result.Content = original.Content;

        return result;
    }

    public IAuthorizedHttpClientMessage BuildPost(string path)
    {
        return new AuthorizedHttpClientMessage(this, HttpMethod.Post, path);
    }
    public IAuthorizedHttpClientMessage BuildGet(string path)
    {
        return new AuthorizedHttpClientMessage(this, HttpMethod.Get, path);
    }
    public IAuthorizedHttpClientMessage BuildDelete(string path)
    {
        return new AuthorizedHttpClientMessage(this, HttpMethod.Delete, path);
    }

    public class AuthorizedHttpClientMessage : IAuthorizedHttpClientMessage
    {
        readonly IAuthorizedHttpClient _authorizedHttpClient;
        public bool IsImpersonated { get; private set; }
        public HttpMethod Method { get; private set; }
        public Uri Uri { get; private set; }
        public StringContent? Content { get; private set; } = null;

        public AuthorizedHttpClientMessage(IAuthorizedHttpClient authorizedHttpClient, HttpMethod method, string path)
        {
            _authorizedHttpClient = authorizedHttpClient;
            Method = method;
            Uri = new Uri(_authorizedHttpClient.ApplicationUri, path);
        }

        public IAuthorizedHttpClientMessage WithImpersonated()
        {
            IsImpersonated = true;
            return this;
        }

        public IAuthorizedHttpClientMessage WithContent(object content)
        {
            Content = new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json");
            return this;
        }

        public async Task SendAsync(CancellationToken cancellationToken)
        {
            await _authorizedHttpClient.SendAsync<AuthorizedHttpClientMockResult>(this.Method, this.Uri, this.Content, this.IsImpersonated, cancellationToken);
        }

        public async Task<T> SendWithResulAsync<T>(CancellationToken cancellationToken)
        {
            return await _authorizedHttpClient.SendAsync<T>(this.Method, this.Uri, this.Content, this.IsImpersonated, cancellationToken);
        }
    }

    private struct AuthorizedHttpClientMockResult;
}
