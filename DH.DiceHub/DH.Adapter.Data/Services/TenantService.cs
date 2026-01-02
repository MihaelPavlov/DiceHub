using DH.Domain.Adapters.Data;
using DH.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DH.Adapter.Data.Services;

public class TenantService(TenantDbContext db) : ITenantService
{
    private readonly TenantDbContext _db = db;

    public async Task<Tenant?> GetByTenantName(string name)
    {
        return await _db.Tenants
            .Where(t => t.TenantName == name)
            .FirstOrDefaultAsync();
    }
}
