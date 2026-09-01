using DH.Domain.Entities;

namespace DH.Domain.Adapters.Data;

/// <summary>
/// Provides tenant-related data access operations.
/// </summary>
/// <remarks>
/// This service acts as an abstraction over <see cref="TenantDbContext"/> 
/// for querying tenant information.
/// </remarks>
public interface ITenantService
{
    /// <summary>
    /// Retrieves a tenant by its unique name (tenant name).
    /// </summary>
    /// <param name="name">
    /// The tenant name used to identify the tenant (usually taken from the route).
    /// </param>
    /// <returns>
    /// The matching <see cref="Tenant"/> if found; otherwise, <c>null</c>.
    /// </returns>
    Task<Tenant?> GetByTenantName(string name);
}
