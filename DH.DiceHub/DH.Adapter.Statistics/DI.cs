using DH.Domain.Adapters.Statistics;
using Microsoft.Extensions.DependencyInjection;

namespace DH.Adapter.Statistics;

public static class DI
{
    public static IServiceCollection AddStatisticsAdapter(this IServiceCollection services)
        => services
       .AddScoped<IStatisticJobFactory, StatisticJobFactory>()
       .AddScoped<IStatisticJobQueue, StatisticJobQueue>()
       .AddHostedService<StatisticJobWorker>();
}
