
using Autofac;
using Autofac.Extensions.DependencyInjection;
using DH.Adapter.Authentication;
using DH.Adapter.Authentication.Helper;
using DH.Adapter.Data;
using DH.Adapter.Data.Services;
using DH.Adapter.QRManager;
using DH.Adapter.Scheduling;
using DH.Api;
using DH.Api.Helpers;
using DH.Application;
using DH.Application.Games.Commands.Games;
using DH.Database.MigrationUtility;
using DH.Domain;
using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Data;
using DH.Domain.Helpers;
using DH.Domain.Services;
using DH.Domain.Services.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;

using var logger = new ConsoleFileLogger();
try
{
    var builder = new ConfigurationBuilder()
        .AddJsonFile($"appsettings.json", true, false);
    var configuration = builder.Build();

    var connectionString = configuration.GetConnectionString("DefaultConnection");
    if (connectionString == null)
    {
        logger.WriteLine("ConnectionString is expected");
        return 1;
    }

    //var environmentSettings = configuration.GetSection("Environment").Get<EnvironmentSettings>();
    //if (environmentSettings == null)
    //{
    //    logger.WriteLine("Environment is expected");
    //    return 1;
    //}

    await MigrateShareDatabases(connectionString, logger);
    await MigrateTenantDatabases(connectionString, logger);

    var host = Host
        .CreateDefaultBuilder()
        .ConfigureLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddConsole();
        })
        .ConfigureServices((hostContext, services) =>
        {
            // Configuration
            var connectionShared = configuration.GetConnectionString("DefaultConnection"); ;
            if (string.IsNullOrEmpty(connectionShared))
                throw new Exception("ConnectionStrings BaseConnectionString is expected in appsettings.json");

            services.AddScoped<IDataSeeder, DataSeeder>();
            services.AddAutofac();

            services.LoadDatabase(hostContext.Configuration);
            services.AuthenticationAdapter(hostContext.Configuration);
            services.AddSchedulingConfiguration(hostContext.Configuration);
            services.LoadSeedServices();
            services.ConfigureQrCodeManager();
            services.AddScoped<IContainerService, ContainerService>();
            services.AddScoped<IRewardService, RewardService>();
            services.AddScoped<IChallengeService, ChallengeService>();
            services.AddScoped<IGameService, GameService>();
            //services.AddAutofac();
            var wwwrootPath = ProjectPathHelper.BuildWebRootPath();
            services.AddScoped<IWebRootPathHelper>(provider => new NonWebRootPathHelper(wwwrootPath));
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(CreateGameCommand).Assembly));
            services.AddScoped<IUserContextFactory, VirtualUserContextFactory>();
            services.AddScoped<IUserContext>(services => services.GetRequiredService<IUserContextFactory>().CreateUserContext());
        })
        .Build();

    host.SeedUsersAsync();
    using (var scope = host.Services.CreateScope())
    {
        var dataSeeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
        await dataSeeder.SeedAsync();
    }
}
catch (Exception ex)
{
    logger.WriteLine();
    logger.WriteLine();
    logger.WriteLine(ex.ToString());
    return 1;
}
finally
{
    logger.MentionHerself();
}

return 0;

static async Task MigrateTenantDatabases(string connectionString, ConsoleFileLogger logger)
{
    logger.WriteLine();
    logger.WriteLine("-------------------------------------------------------------");
    logger.WriteLine("Start migrations for Tenant database");
    logger.WriteLine("-------------------------------------------------------------");
    logger.WriteLine();

    var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
    optionsBuilder
        .UseSqlServer(connectionString, x => x.MigrationsAssembly("DH.Adapter.Data"))
        .LogTo(logger.WriteLine, LogLevel.Information);

    using (var context = new TenantDbContext(optionsBuilder.Options))
    {
        await context.Database.MigrateAsync();
    }
}
static async Task MigrateShareDatabases(string connectionString, ConsoleFileLogger logger)
{
    logger.WriteLine();
    logger.WriteLine("-------------------------------------------------------------");
    logger.WriteLine("Start migrations for Shared database");
    logger.WriteLine("-------------------------------------------------------------");
    logger.WriteLine();

    var optionsBuilder = new DbContextOptionsBuilder<AppIdentityDbContext>();
    optionsBuilder
        .UseSqlServer(connectionString, x => x.MigrationsAssembly("DH.Adapter.Authentication"))
        .LogTo(logger.WriteLine, LogLevel.Information);

    using (var context = new AppIdentityDbContext(optionsBuilder.Options))
    {
        await context.Database.MigrateAsync();
    }
}

public class NonWebRootPathHelper : IWebRootPathHelper
{
    private readonly string _defaultRootPath;

    public NonWebRootPathHelper(string defaultRootPath)
    {
        _defaultRootPath = defaultRootPath;
    }

    public string GetWebRootPath => _defaultRootPath;

}