using DH.Messaging.HttpClient.Enums;

namespace DH.Messaging.HttpClient;

public interface IAuthorizedClientFactory
{
    IAuthorizedHttpClient CreateClient(ApplicationApi application, string userId, string token);
}
