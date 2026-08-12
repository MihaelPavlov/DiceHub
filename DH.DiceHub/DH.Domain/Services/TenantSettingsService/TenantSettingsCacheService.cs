using DH.Domain.Entities;
using DH.Domain.Adapters.Authentication;
using DH.Domain.Repositories;

namespace DH.Domain.Services.TenantSettingsService;

internal class TenantSettingsCacheService : ITenantSettingsCacheService
{
    readonly ReaderWriterLockSlim cacheLock = new();
    private TenantSetting? _cache;
    private string? _cacheTenantId;
    DateTime _cacheDateTime = DateTime.MinValue;
    IRepository<TenantSetting> repository;
    IRepository<Tenant> tenantRepository;
    IUserContext userContext;

    public TenantSettingsCacheService(
        IRepository<TenantSetting> repository,
        IRepository<Tenant> tenantRepository,
        IUserContext userContext)
    {
        this.repository = repository;
        this.tenantRepository = tenantRepository;
        this.userContext = userContext;
    }

    public async Task<TenantSetting> GetGlobalTenantSettingsAsync(CancellationToken cancellationToken)
    {
        var tenantId = userContext.TenantId == "system" ? null : userContext.TenantId;

        if (_cache is null || _cacheTenantId != tenantId || _cacheDateTime.AddMinutes(3) < DateTime.UtcNow)
        {
            TenantSetting? tenantSettings;

            if (!string.IsNullOrWhiteSpace(tenantId))
            {
                tenantSettings = (await this.tenantRepository.GetWithPropertiesAsync(
                    x => x.Id == tenantId,
                    x => x.TenantSetting,
                    cancellationToken)).FirstOrDefault();
            }
            else
            {
                tenantSettings = await this.repository.GetByAsync(x => x.Id == 1, cancellationToken);
            }

            if (tenantSettings == null)
                throw new InvalidOperationException(
                    string.IsNullOrWhiteSpace(tenantId)
                        ? "Global tenant settings (Id = 1) not found."
                        : $"Tenant settings for '{tenantId}' were not found.");

            cacheLock.EnterWriteLock();
            try
            {
                _cacheDateTime = DateTime.UtcNow;
                _cache = tenantSettings;
                _cacheTenantId = tenantId;
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }

        cacheLock.EnterReadLock();
        try
        {
            return _cache ?? new TenantSetting();
        }
        finally
        {
            cacheLock.ExitReadLock();
        }
    }
}
