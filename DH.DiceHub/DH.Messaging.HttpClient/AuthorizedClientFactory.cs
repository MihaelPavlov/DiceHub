using DH.Messaging.HttpClient.Enums;
using DH.Messaging.HttpClient.Helpers;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace DH.Messaging.HttpClient;

public class AuthorizedClientFactory : IAuthorizedClientFactory
{
    readonly ILogger<AuthorizedClientFactory> _logger;
    readonly IHttpClientFactory _httpClientFactory;

    public AuthorizedClientFactory(ILogger<AuthorizedClientFactory> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public IAuthorizedHttpClient CreateClient(ApplicationApi application, string userId, string token)
    {
        var httpClientName = application switch
        {
            ApplicationApi.Statistics => HttpClientNames.Statistics,
        };
        var applicationUri = new Uri(ApplicationUrlHelper.GetApplicationUrl(application));

        return new AuthorizedHttpClient(application, httpClientName, userId, token, applicationUri, _httpClientFactory, _logger);
    }
}

