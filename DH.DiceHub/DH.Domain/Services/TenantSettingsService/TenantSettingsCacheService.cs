using DH.Domain.Entities;
using DH.Domain.Repositories;

namespace DH.Domain.Services.TenantSettingsService;

internal class TenantSettingsCacheService : ITenantSettingsCacheService
{
    readonly ReaderWriterLockSlim cacheLock = new();
    private TenantSetting _cache;
    DateTime _cacheDateTime = DateTime.MinValue;
    IRepository<TenantSetting> repository;

    public TenantSettingsCacheService(IRepository<TenantSetting> repository)
    {
        this.repository = repository;
    }

    public async Task<TenantSetting> GetGlobalTenantSettingsAsync(CancellationToken cancellationToken)
    {
        if (_cacheDateTime.AddMinutes(3) < DateTime.UtcNow)
        {
            var tenantSettings = await this.repository.GetByAsync(x => x.Id == 1, cancellationToken);

            cacheLock.EnterWriteLock();
            try
            {
                _cacheDateTime = DateTime.UtcNow;
                _cache = tenantSettings;
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }

        cacheLock.EnterReadLock();
        try
        {
            return _cache;
        }
        finally
        {
            cacheLock.ExitReadLock();
        }
    }
}
