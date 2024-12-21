using Microsoft.Extensions.DependencyInjection;

namespace DH.Messaging.HttpClient;

public static class DI
{
    public static void AddAuthenticationService(this IServiceCollection services)
    {
        services.AddScoped<IAuthorizedClientFactory, AuthorizedClientFactory>();
    }
}
