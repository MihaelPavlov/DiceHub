using DH.Domain.Adapters.PushNotifications;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.DependencyInjection;

namespace DH.Adapter.PushNotifications;

public static class DI
{
    public static IServiceCollection AddFirebaseMessaging(this IServiceCollection services)
    {
       return services.AddScoped<IPushNotificationsService, PushNotificationsService>();
    }
}
