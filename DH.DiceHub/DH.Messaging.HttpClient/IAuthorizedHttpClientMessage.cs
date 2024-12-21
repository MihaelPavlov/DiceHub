namespace DH.Messaging.HttpClient;

public interface IAuthorizedHttpClientMessage
{
    IAuthorizedHttpClientMessage WithImpersonated();
    IAuthorizedHttpClientMessage WithContent(object content);
    Task SendAsync(CancellationToken cancellationToken);
    Task<T> SendWithResulAsync<T>(CancellationToken cancellationToken);
}
