using DH.Messaging.HttpClient.UserContext;
using Microsoft.Extensions.DependencyInjection;

namespace DH.Authentication.UserContext;

public static class DI
{
    public static void AddUserContextService(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IUserContextFactory, UserContextFactory>();
        services.AddScoped<IUserContext>(services => services.GetRequiredService<IUserContextFactory>().CreateUserContext());
    }
}
