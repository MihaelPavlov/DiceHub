using DH.Statistics.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DH.Statistics.Application;

public static class DI
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration) 
        => services
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(CreateClubVisitorLogCommand).Assembly))
            .AddStatisticsData(configuration);
}
