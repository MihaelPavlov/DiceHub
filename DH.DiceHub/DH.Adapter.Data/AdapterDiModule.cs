using DH.Adapter.Data.Repositories;
using DH.Adapter.Data.Seeder;
using DH.Adapter.Data.Services;
using DH.Domain.Adapters.Data;
using DH.Domain.Adapters.Statistics.Services;
using DH.Domain.Repositories;
using DH.Domain.Seeder;
using DH.Domain.Services;
using DH.Domain.Services.Queue;
using DH.Domain.Services.TenantUserSettingsService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DH.Adapter.Data;

public static class DataDIModule
{
    public static IServiceCollection AddDataAdapter(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // Add or override pooling settings
        var builder = new Npgsql.NpgsqlConnectionStringBuilder(connectionString)
        {
            Pooling = true,            // enable connection pooling
            MaxPoolSize = 25,          // max concurrent connections from pool (adjust as needed)
            MinPoolSize = 5,           // optional: minimum pool size
            Timeout = 15               // optional: connection timeout in seconds
        };

        services
            .AddDbContext<TenantDbContext>(options => options
                .UseNpgsql(
                    builder.ConnectionString,
                    npgsqlOptions => 
                        npgsqlOptions
                        .EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(5),
                            errorCodesToAdd: null)
                        .MigrationsAssembly(typeof(TenantDbContext).Assembly.FullName))
            .EnableDetailedErrors(true)) // TODO:Don't do this on production. Don't hardcode this
            .AddScoped<ITenantDbContext>(provider => provider.GetService<TenantDbContext>()
                ?? throw new ArgumentNullException("IDBContext was not found"))
            .AddScoped(typeof(IRepository<>), typeof(DataRepository<>))
            .AddScoped<IUserChallengesManagementService, UserChallengesManagementService>()
            .AddScoped<IGameSessionService, GameSessionService>()
            .AddScoped<IChallengeService, ChallengeService>()
            .AddScoped<IEventService, EventService>()
            .AddScoped<IGameCategoryService, GameCategoryService>()
            .AddScoped<IGameService, GameService>()
            .AddScoped<IRewardService, RewardService>()
            .AddScoped<IRoomService, RoomService>()
            .AddScoped<ISpaceTableService, SpaceTableService>()
            .AddScoped<IQueuedJobService, QueuedJobService>()
            .AddScoped<IDataSeeder, DataSeeder>()
            .AddScoped<IEmailHelperService, EmailHelperService>()
            .AddScoped<IStatisticsService, StatisticsService>()
            .AddScoped<IUniversalChallengeProcessing, UniversalChallengeProcessing>()
            .AddScoped<IGameSeeder, GameSeeder>()
            .AddScoped<ITenantResolver, TenantResolver>()
            .AddSingleton<TenantDbConnectionInterceptor>();

        services.AddMemoryCache();
        services.AddScoped<IUserSettingsCache, UserSettingsCache>();

        RegisterAssemblyTypesAsClosedGeneric(services, typeof(IDomainService<>), typeof(IDbContextFactory<>));
        return services;
    }

    //TODO: VERIFY IF THIS IS NEEDED
    private static void RegisterAssemblyTypesAsClosedGeneric(IServiceCollection services, params Type[] openGenericInterfaces)
    {
        var assembly = Assembly.GetExecutingAssembly(); // Assuming the current assembly contains your types

        var types = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract) // Only get non-abstract classes
            .ToList();

        foreach (var type in types)
        {
            foreach (var openGenericInterface in openGenericInterfaces)
            {
                // Find interfaces that close the open generic interface
                var closedGenericInterfaces = type.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == openGenericInterface)
                    .ToList();

                foreach (var closedGenericInterface in closedGenericInterfaces)
                {
                    services.AddScoped(closedGenericInterface, type);
                }
            }
        }
    }
}

