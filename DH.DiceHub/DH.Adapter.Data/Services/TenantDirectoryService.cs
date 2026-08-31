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

    public async Task<List<TenantScheduleInfo>> GetActiveTenantWorkingHoursAsync(CancellationToken cancellationToken)
    {
        using var context = await this.contextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Tenants
            .Where(t => t.TenantStatus == TenantStatus.Active)
            .Select(t => new TenantScheduleInfo(
                t.Id,
                t.TenantSetting.EndWorkingHours,
                t.TenantSetting.TimeZoneId,
                t.TenantSetting.PeriodOfRewardReset,
                t.TenantSetting.ResetDayForRewards))
            .ToListAsync(cancellationToken);
    }

    public async Task<TenantScheduleInfo?> GetTenantScheduleInfoAsync(string tenantId, CancellationToken cancellationToken)
    {
        using var context = await this.contextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Tenants
            .Where(t => t.Id == tenantId)
            .Select(t => new TenantScheduleInfo(
                t.Id,
                t.TenantSetting.EndWorkingHours,
                t.TenantSetting.TimeZoneId,
                t.TenantSetting.PeriodOfRewardReset,
                t.TenantSetting.ResetDayForRewards))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
