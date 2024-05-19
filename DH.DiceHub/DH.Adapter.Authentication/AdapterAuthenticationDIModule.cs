using Autofac;
using DH.Adapter.Authentication.Services;
using DH.Domain.Repositories;
using DH.Domain.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DH.Adapter.Authentication.Helper;
using DH.Domain.Adapters.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Module = Autofac.Module;

namespace DH.Adapter.Authentication;

public class AdapterAuthenticationDIModule : Module
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

        builder.RegisterType<AppIdentityDbContext>().As<IIdentityDbContext>().InstancePerLifetimeScope();
    }
}

public static class AuthenticationDIModule
{
    public static void SeedUsersAsync(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            try
            {
                ApplicationDbContextSeeder.SeedUsers(scope.ServiceProvider).Wait();
            }
            catch (Exception ex)
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<WebApplication>>();
                logger.LogError(ex, "An error occurred seeding the database.");
            }
        }

    }

    public static IServiceCollection AuthenticationAdapter(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppIdentityDbContext>(x =>
            x.UseSqlServer(
                connectionString: configuration.GetConnectionString("DefaultConnection"),
                sqlServer => sqlServer
                    .MigrationsAssembly(typeof(AppIdentityDbContext).Assembly.FullName)
            )
        );

        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<AppIdentityDbContext>()
            .AddDefaultTokenProviders();

        services
            .AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var fe_url = configuration.GetValue<string>("Front_End_Application_URL")
                    ?? throw new ArgumentException("Front_End_Application_URL was not specified");

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = fe_url,
                    ValidAudience = fe_url,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("JWT_SecrectKey")
                        ?? throw new ArgumentException("JWT_SecretKey was not specified")))
                };
            });

        services.AddAuthorization();

        services
           .AddScoped<IUserService, UserService>()
           .AddScoped<IUserContextFactory, UserContextFactory>()
           .AddScoped<IUserContext>(services => services.GetRequiredService<IUserContextFactory>().CreateUserContext());

        return services;
    }
}
