using DH.Domain.Entities;

namespace DH.Domain.Services.TenantSettingsService;

public interface ITenantSettingsCacheService
{
    Task<TenantSetting> GetGlobalTenantSettingsAsync(CancellationToken cancellationToken);
}