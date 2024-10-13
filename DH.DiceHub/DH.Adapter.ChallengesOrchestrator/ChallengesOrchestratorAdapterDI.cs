using DH.Domain.Adapters.ChallengesOrchestrator;
using Microsoft.Extensions.DependencyInjection;

namespace DH.Adapter.ChallengesOrchestrator;

public static class ChallengesOrchestratorAdapterDI
{
    public static IServiceCollection AddChallengesOrchestratorAdapter(
        this IServiceCollection services)
        => services
            .AddSingleton<SynchronizeUsersChallengesQueue>()
            .AddHostedService<SynchronizeUsersChallengesService>();
}
