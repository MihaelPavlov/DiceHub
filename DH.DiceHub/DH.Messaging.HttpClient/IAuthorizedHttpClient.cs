namespace DH.Messaging.HttpClient;

public interface IAuthorizedHttpClient
{
    Uri ApplicationUri { get; }

    Task<TResult> SendAsync<TResult>(HttpMethod method, Uri uri, StringContent? content, bool isImpersonated, CancellationToken cancellationToken);  

    IAuthorizedHttpClientMessage BuildPost(string path);
    IAuthorizedHttpClientMessage BuildGet(string path);
    IAuthorizedHttpClientMessage BuildDelete(string path);
}
