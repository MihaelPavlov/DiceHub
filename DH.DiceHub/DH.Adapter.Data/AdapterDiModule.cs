using Autofac;
using DH.Adapter.Data.Repositories;
using DH.Adapter.Data.Services;
using DH.Domain.Adapters.Data;
using DH.Domain.Repositories;
using DH.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DH.Adapter.Data;

public class AdapterDataDIModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(this.ThisAssembly)
         .AsClosedTypesOf(typeof(IDomainService<>))
         .AsImplementedInterfaces()
        .InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(this.ThisAssembly)
              .AsClosedTypesOf(typeof(IDbContextFactory<>))
              .AsImplementedInterfaces()
             .InstancePerLifetimeScope();

        builder.RegisterType<TenantDbContext>().As<ITenantDbContext>().InstancePerLifetimeScope();
    }
}
public static class DataDIModule
{
    public static IServiceCollection LoadDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
        => services
            .AddDbContext<TenantDbContext>(options => options
                .UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sqlServer => sqlServer
                        .MigrationsAssembly(typeof(TenantDbContext).Assembly.FullName))
            .EnableDetailedErrors(true)) // TODO:Don't do this on production. Don't hardcode this
            .AddScoped<ITenantDbContext>(provider => provider.GetService<TenantDbContext>()
                ?? throw new ArgumentNullException("IDBContext was not found"))
            .AddScoped(typeof(IRepository<>), typeof(DataRepository<>))
            .AddScoped<IUserChallengesManagementService, UserChallengesManagementService>();
}

