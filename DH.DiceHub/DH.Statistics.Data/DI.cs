using DH.Database.Connector;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DH.Statistics.Data;

public static class DI
{
    public static IServiceCollection AddStatisticsData(this IServiceCollection services, IConfiguration configuration)
    {
        var assembly = Assembly.GetAssembly(typeof(StatisticsDbContext))
            ?? throw new InvalidOperationException("StatisticsDbContext assembly was not found");

        services.AddDatabaseConnector<StatisticsDbContext>(configuration, assembly);

        return services;
    }
}
