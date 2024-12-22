using DH.Messaging.HttpClient.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace DH.Messaging.HttpClient;

public static class DI
{
    public static void AddCommunicationService(this IServiceCollection services)
    {
        //for dev
        services.AddHttpClient(HttpClientNames.Statistics, options => { })
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
                });

        // for prod
        // services.AddHttpClient(HttpClientNames.Statistics);
        services.AddScoped<IAuthorizedClientFactory, AuthorizedClientFactory>();
    }
}
