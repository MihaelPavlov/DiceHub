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
using DH.Adapter.Authentication.Entities;
using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.Authentication.Interfaces;
using DH.Domain.Adapters.Authentication.Enums;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace DH.Adapter.Authentication;

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

    public static void SeedUsersAsync(this IHost app)
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
            x.UseNpgsql(
                connectionString: configuration.GetConnectionString("DefaultConnection"),
                sqlServer => sqlServer
                    .MigrationsAssembly(typeof(AppIdentityDbContext).Assembly.FullName)
            )
        );
        services.AddScoped<IIdentityDbContext, AppIdentityDbContext>();

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 0;
            })
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
                var apiAudiences = configuration.GetSection("APIs_Audience_URLs").Get<string[]>()
                    ?? throw new ArgumentException("APIs_Audience_URLs was not specified");

                var issuer = configuration.GetValue<string>("TokenIssuer")
                    ?? throw new ArgumentException("TokenIssuer was not specified");

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudiences = apiAudiences,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("JWT_SecretKey")
                        ?? throw new ArgumentException("JWT_SecretKey was not specified")))
                };
            });

        services.AddAuthorization();

        services.AddHttpClient().AddHttpContextAccessor();
        services
           .AddScoped<IUserService, UserService>()
           .AddScoped<IJwtService, JwtService>()
           .AddScoped<IUserActionService, UserActionService>()
           .AddScoped<IPermissionStringBuilder, PermissionStringBuilder>()
           .AddScoped<IMapPermissions, MapPermissions>()
           .AddScoped<IActionPermissions<UserAction>, MapPermissions>()
           .AddScoped<IUserContextFactory, UserContextFactory>()
           .AddScoped<IUserContext>(services => services.GetRequiredService<IUserContextFactory>().CreateUserContext());

        RegisterAssemblyTypesAsClosedGeneric(services, typeof(IRepository<>), typeof(IDomainService<>), typeof(IDbContextFactory<>));

        return services;
    }

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
                    // Register each type as its closed generic interface with Scoped lifetime
                    services.AddScoped(closedGenericInterface, type);
                }
            }
        }
    }
}
