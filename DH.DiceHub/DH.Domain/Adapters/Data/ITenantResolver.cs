using DH.Domain.Entities;

namespace DH.Domain.Adapters.Data;

public interface ITenantResolver
{
    /// <summary>
    /// Returns tenant info by its slug / route name.
    /// </summary>
    Task<Tenant?> GetBySlugAsync(string slug);
}
