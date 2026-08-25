namespace DH.Domain.Services;

/// <summary>
/// Runs a unit of work under a specific tenant's system context, for use by
/// background jobs and workers that iterate tenants outside of an HTTP request.
/// Tenant-scoped EF query filters and Postgres RLS both key off the ambient
/// user context, so any tenant-scoped read/write performed inside <paramref name="action"/>
/// must happen inside this scope to see (or affect) the right tenant's rows.
/// </summary>
public interface ITenantContextScopeRunner
{
    Task RunAsTenantAsync(string tenantId, Func<Task> action);

    Task<T> RunAsTenantAsync<T>(string tenantId, Func<Task<T>> action);
}
