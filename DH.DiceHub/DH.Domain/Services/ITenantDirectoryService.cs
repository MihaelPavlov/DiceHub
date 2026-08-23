namespace DH.Domain.Services;

/// <summary>
/// Enumerates tenants for background jobs/workers that must process every tenant
/// in turn. The Tenants table itself is not tenant-scoped, so this is safe to call
/// with no ambient tenant/system context.
/// </summary>
public interface ITenantDirectoryService
{
    Task<List<string>> GetActiveTenantIdsAsync(CancellationToken cancellationToken);
}
