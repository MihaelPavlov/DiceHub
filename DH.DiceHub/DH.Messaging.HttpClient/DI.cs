using DH.Messaging.HttpClient.Enums;
using DH.Messaging.HttpClient.UserContext;
using Microsoft.Extensions.DependencyInjection;

namespace DH.Messaging.HttpClient;

public static class DI
{
    public static void AddAuthenticationService(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IB2bUserContextFactory, B2bUserContextFactory>();
        services.AddScoped<IB2bUserContext>(services => services.GetRequiredService<IB2bUserContextFactory>().CreateUserContext());
    }

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
