using DH.Domain.Adapters.GameSession;
using Microsoft.Extensions.DependencyInjection;

namespace DH.Adapter.GameSession;

public static class GameSessionAdapterDI
{
    public static IServiceCollection AddGameSessionAdapter(
        this IServiceCollection services)
        => services
            .AddSingleton<SynchronizeGameSessionQueue>()
            .AddHostedService<SynchronizeGameSessionService>();
}
