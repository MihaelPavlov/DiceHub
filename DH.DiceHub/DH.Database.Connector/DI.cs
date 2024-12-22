using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DH.Database.Connector;

public static class DI
{
    public static IServiceCollection AddDatabaseConnector<TDbContext>(this IServiceCollection services, IConfiguration configuration, Assembly entityAssembly)
        where TDbContext : TenantDbContext
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string not found");

        services.AddDbContext<TDbContext>(
            options => options.UseSqlServer(connectionString,
            sqlServer => sqlServer.MigrationsAssembly(typeof(TDbContext).Assembly.FullName)));

        services.AddSingleton<IDbContextFactory<TDbContext>>(
            new TenantDbContextFactory<TDbContext>(connectionString, entityAssembly));
        services.AddSingleton(provider =>
        {
            var assembly = Assembly.GetExecutingAssembly();  // You can adjust this if the entities are in a different assembly
            return assembly;
        });
        //services.AddScoped<TDbContext>(provider =>
        //{
        //    var options = provider.GetRequiredService<DbContextOptions<TDbContext>>()
        //        ?? throw new InvalidOperationException("Db Context Options not found");
        //    // Use the provided assembly
        //    var assembly = entityAssembly
        //        ?? throw new InvalidOperationException("Entity assembly not provided");

        //    var dbContext = (TDbContext?)Activator.CreateInstance(typeof(TDbContext), options, assembly);

        //    if (dbContext == null)
        //        throw new InvalidOperationException($"Db context from instance {typeof(TDbContext).Name} was not found");

        //    return dbContext;
        //});
         return services;
    }
}
