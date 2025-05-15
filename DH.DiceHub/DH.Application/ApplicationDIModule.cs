using DH.Application.Games.Commands.Games;
using DH.Application.Statistics;
using DH.Domain.Adapters.Statistics;
using Microsoft.Extensions.DependencyInjection;

namespace DH.Application;

public static class ApplicationDIModule
{
    public static void AddApplication(
        this IServiceCollection services)
        => services
            .AddScoped<IStatisticJobFactory, StatisticJobFactory>()
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(CreateGameCommand).Assembly));
}