using DH.Domain.Adapters.Data;
using DH.Domain.Entities;
using DH.OperationResultCore.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DH.Adapter.Data.Services;

public class TenantService(TenantDbContext db) : ITenantService
{
    private readonly TenantDbContext db = db;

    public async Task<Tenant?> GetByTenantName(string name)
    {
        return await this.db.Tenants
            .Include(x => x.TenantSetting)
            .FirstOrDefaultAsync(t => t.TenantName == name);
    }

    public async Task<Tenant?> GetById(string id)
    {
        return await this.db.Tenants
            .Include(x => x.TenantSetting)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Tenant?> GetByRouteIdentifier(string routeIdentifier)
    {
        return await this.GetById(routeIdentifier)
            ?? await this.GetByTenantName(routeIdentifier);
    }

    public async Task<Tenant> GetCurrentTenantAsync(CancellationToken cancellationToken)
    {
        return await this.db.Tenants
            .Include(x => x.TenantSetting)
            .AsTracking()
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(nameof(Tenant));
    }

    public async Task<Tenant> CreateAsync(Tenant tenant, CancellationToken cancellationToken)
    {
        await this.db.Tenants.AddAsync(tenant, cancellationToken);
        await this.db.SaveChangesAsync(cancellationToken);
        return tenant;
    }
}
