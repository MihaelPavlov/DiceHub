using DH.Domain.Queue.Services;
using DH.Domain.Services.TenantSettingsService;
using Microsoft.Extensions.DependencyInjection;

namespace DH.Domain;

public static class DomainDIModule
{
    public static void AddDomain(
        this IServiceCollection services)
        => services
            .AddScoped<ITenantSettingsCacheService, TenantSettingsCacheService>()
            .AddScoped<IQueueDispatcher, QueueDispatcher>();
}
