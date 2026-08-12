using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Data;
using DH.Domain.Entities;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using System.Collections.Concurrent;

namespace DH.Domain.Services.TenantSettingsService;

internal class TenantSettingsCacheService : ITenantSettingsCacheService
{
    readonly ReaderWriterLockSlim cacheLock = new();
    readonly ConcurrentDictionary<string, CacheEntry> cache = new();
    readonly ITenantService tenantService;
    readonly IUserContext userContext;
    readonly ITenantExecutionContextAccessor tenantExecutionContextAccessor;

    public TenantSettingsCacheService(
        IRepository<TenantSetting> repository,
        ITenantService tenantService,
        IUserContext userContext,
        ITenantExecutionContextAccessor tenantExecutionContextAccessor)
    {
        this.tenantService = tenantService;
        this.userContext = userContext;
        this.tenantExecutionContextAccessor = tenantExecutionContextAccessor;
    }

    public async Task<TenantSetting> GetGlobalTenantSettingsAsync(CancellationToken cancellationToken)
    {
        var tenantId = (!string.IsNullOrWhiteSpace(this.userContext.TenantId) ? this.userContext.TenantId : null)
            ?? this.tenantExecutionContextAccessor.TenantId;
        if (string.IsNullOrWhiteSpace(tenantId))
            throw new InvalidOperationException("Tenant context is required to resolve tenant settings.");

        if (!this.cache.TryGetValue(tenantId, out var cacheEntry) || cacheEntry.CachedAt.AddMinutes(3) < DateTime.UtcNow)
        {
            var tenant = await this.tenantService.GetCurrentTenantAsync(cancellationToken);
            var tenantSettings = tenant.TenantSetting
                ?? throw new NotFoundException(nameof(TenantSetting));

            this.cacheLock.EnterWriteLock();
            try
            {
                this.cache[tenantId] = new CacheEntry(tenantSettings, DateTime.UtcNow);
            }
            finally
            {
                this.cacheLock.ExitWriteLock();
            }
        }

        this.cacheLock.EnterReadLock();
        try
        {
            return this.cache.TryGetValue(tenantId, out var cached)
                ? cached.Settings
                : throw new InvalidOperationException("Tenant settings cache was not populated.");
        }
        finally
        {
            this.cacheLock.ExitReadLock();
        }
    }

    public void Clear(string? tenantId = null)
    {
        if (string.IsNullOrWhiteSpace(tenantId))
        {
            this.cache.Clear();
            return;
        }

        this.cache.TryRemove(tenantId, out _);
    }

    private sealed record CacheEntry(TenantSetting Settings, DateTime CachedAt);
}
