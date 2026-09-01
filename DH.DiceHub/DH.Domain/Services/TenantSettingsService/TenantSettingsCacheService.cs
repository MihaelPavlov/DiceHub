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
    IUserContextFactory userContextFactory;

    public TenantSettingsCacheService(
        IRepository<TenantSetting> repository,
        IRepository<Tenant> tenantRepository,
        IUserContextFactory userContextFactory)
    {
        this.repository = repository;
        this.tenantRepository = tenantRepository;
        this.userContextFactory = userContextFactory;
    }

    public async Task<TenantSetting> GetGlobalTenantSettingsAsync(CancellationToken cancellationToken)
    {
        // Resolved fresh on every call rather than taking a constructor-injected
        // IUserContext: IUserContext is Scoped and memoized on first resolution, but
        // this service is itself first resolved (via IUserChallengesManagementService)
        // before ITenantContextScopeRunner.RunAsTenantAsync sets the per-job tenant
        // for background workers that process multiple tenants' jobs within one scope
        // - a cached IUserContext would keep reflecting whatever (or no) tenant was
        // ambient at that first resolution for the rest of the scope's lifetime.
        var userContext = await this.userContextFactory.CreateAsync();
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
