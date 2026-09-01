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

        // Null-safe member access on the navigation so EF emits a LEFT JOIN. A plain
        // `t.TenantSetting.X` projects a required navigation -> INNER JOIN, which
        // silently drops any tenant whose settings row can't be joined, so that
        // tenant never gets its per-tenant scheduled jobs reconciled.
        return await context.Tenants
            .Where(t => t.TenantStatus == TenantStatus.Active)
            .Select(t => new TenantScheduleInfo(
                t.Id,
                t.TenantSetting != null ? t.TenantSetting.EndWorkingHours : string.Empty,
                t.TenantSetting != null ? t.TenantSetting.TimeZoneId : null,
                t.TenantSetting != null ? t.TenantSetting.PeriodOfRewardReset : string.Empty,
                t.TenantSetting != null ? t.TenantSetting.ResetDayForRewards : string.Empty))
            .ToListAsync(cancellationToken);
    }

    public async Task<TenantScheduleInfo?> GetTenantScheduleInfoAsync(string tenantId, CancellationToken cancellationToken)
    {
        using var context = await this.contextFactory.CreateDbContextAsync(cancellationToken);

        // Null-safe member access -> LEFT JOIN (see GetActiveTenantWorkingHoursAsync).
        return await context.Tenants
            .Where(t => t.Id == tenantId)
            .Select(t => new TenantScheduleInfo(
                t.Id,
                t.TenantSetting != null ? t.TenantSetting.EndWorkingHours : string.Empty,
                t.TenantSetting != null ? t.TenantSetting.TimeZoneId : null,
                t.TenantSetting != null ? t.TenantSetting.PeriodOfRewardReset : string.Empty,
                t.TenantSetting != null ? t.TenantSetting.ResetDayForRewards : string.Empty))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
