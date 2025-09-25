using DH.Domain.Adapters.ChallengeHub;
using DH.Domain.Adapters.PushNotifications;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace DH.Adapter.ChallengeHub;

public static class ChallengeHubDIModule
{
    public static void AddChallengeHubAdapter(
        this IServiceCollection services)
            => services.AddScoped<IChallengeHubClient>(sp =>
            {
                var hubContext = sp.GetRequiredService<IHubContext<ChallengeHubClient>>();
                var pushNotifications = sp.GetRequiredService<IPushNotificationsService>();
                return new ChallengeHubClientProxy(hubContext, pushNotifications);
            });
}
