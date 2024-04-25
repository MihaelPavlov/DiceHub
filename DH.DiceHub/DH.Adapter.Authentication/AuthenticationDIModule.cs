using DH.Adapter.Authentication.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DH.Adapter.Authentication
{
    public static class AuthenticationDIModule
    {
        public static IServiceCollection AuthenticationAdapter(this IServiceCollection services)
        {
            services
               .AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
