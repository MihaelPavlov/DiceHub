using Autofac;
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
            .AsClosedTypesOf(typeof(IRepository<>))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(this.ThisAssembly)
         .AsClosedTypesOf(typeof(IDomainService<>))
         .AsImplementedInterfaces()
        .InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(this.ThisAssembly)
              .AsClosedTypesOf(typeof(IDbContextFactory<>))
              .AsImplementedInterfaces()
             .InstancePerLifetimeScope();

        builder.RegisterType<MyDbContext>().As<IDBContext>().InstancePerLifetimeScope();
    }
}
public static class AdapterService
{
    public static IServiceCollection LoadDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
        => services
            .AddDbContext<MyDbContext>(options => options
                .UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sqlServer => sqlServer
                        .MigrationsAssembly(typeof(MyDbContext).Assembly.FullName))
            .EnableDetailedErrors(true)) // TODO:Don't do this on production. Don't hardcode this
            .AddScoped<IDBContext>(provider => provider.GetService<MyDbContext>()
                ?? throw new ArgumentNullException("IDBContext was not found"));

}

