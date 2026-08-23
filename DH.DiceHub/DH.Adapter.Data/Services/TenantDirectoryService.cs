using DH.Domain.Enums;
using DH.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace DH.Adapter.Data.Services;

public class TenantDirectoryService : ITenantDirectoryService
{
    readonly IDbContextFactory<TenantDbContext> contextFactory;

    public TenantDirectoryService(IDbContextFactory<TenantDbContext> contextFactory)
    {
        this.contextFactory = contextFactory;
    }

    public async Task<List<string>> GetActiveTenantIdsAsync(CancellationToken cancellationToken)
    {
        using var context = await this.contextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Tenants
            .Where(t => t.TenantStatus == TenantStatus.Active)
            .Select(t => t.Id)
            .ToListAsync(cancellationToken);
    }
}
