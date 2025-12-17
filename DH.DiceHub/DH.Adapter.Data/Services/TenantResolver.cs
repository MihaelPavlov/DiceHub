using DH.Domain.Adapters.Data;
using DH.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DH.Adapter.Data.Services;

public class TenantResolver(TenantDbContext db) : ITenantResolver
{
    private readonly TenantDbContext _db = db;

    public async Task<Tenant?> GetBySlugAsync(string slug)
    {
        return await _db.Tenants
            .Where(t => t.TenantName == slug)
            .FirstOrDefaultAsync();
    }
}
