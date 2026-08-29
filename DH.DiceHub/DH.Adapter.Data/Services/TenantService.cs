using DH.Domain.Adapters.Data;
using DH.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace DH.Adapter.Data.Services;

public class TenantService(TenantDbContext db, IMemoryCache cache) : ITenantService
{
    private readonly TenantDbContext _db = db;
    private readonly IMemoryCache _cache = cache;

    // GetByTenantName runs on essentially every authenticated request (via
    // TenantRouteValidationMiddleware), which only reads tenant.Id - a name->id
    // mapping that is immutable for a tenant's lifetime. Cache it to drop a query
    // (and its connection-open / RLS round-trips) from the hot path.
    // AbsoluteExpirationRelativeToNow, not sliding, so a stale entry can't live
    // forever. Nulls are deliberately not cached so a not-yet-created tenant
    // slug isn't negatively cached until the TTL expires.
    // NOTE: if the middleware ever starts reading TenantStatus / LogoFileName off
    // this result, add explicit invalidation (TenantSetupService, UpdateTenantLogoCommand)
    // and/or shorten the TTL.
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);

    public async Task<Tenant?> GetByTenantName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        if (_cache.TryGetValue(CacheKey(name), out Tenant? cached))
            return cached;

        // AsNoTracking: the returned entity is cached and handed to other
        // requests/threads, so it must not be attached to this scoped context.
        var tenant = await _db.Tenants
            .AsNoTracking()
            .Where(t => t.Id == name || t.TenantName == name)
            .FirstOrDefaultAsync();

        if (tenant is null)
            return null;

        _cache.Set(CacheKey(name), tenant, CacheTtl);
        // Also cache under the resolved Id so a later lookup by the other key hits.
        if (!string.Equals(tenant.Id, name, StringComparison.Ordinal))
            _cache.Set(CacheKey(tenant.Id), tenant, CacheTtl);

        return tenant;
    }

    private static string CacheKey(string name) => $"tenant:name:{name}";
}
