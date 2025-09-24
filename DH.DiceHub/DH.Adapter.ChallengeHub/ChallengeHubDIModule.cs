using DH.Domain.Adapters.ChallengeHub;
using Microsoft.Extensions.DependencyInjection;

namespace DH.Adapter.ChallengeHub;

public static class ChallengeHubDIModule
{
    public static void AddChallengeHubAdapter(
        this IServiceCollection services)
            => services
                .AddScoped<IChallengeHubClient, ChallengeHubClient>();
}
