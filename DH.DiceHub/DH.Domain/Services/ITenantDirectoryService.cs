namespace DH.Domain.Services;

/// <summary>
/// Enumerates tenants for background jobs/workers that must process every tenant
/// in turn. The Tenants table itself is not tenant-scoped, so this is safe to call
/// with no ambient tenant/system context.
/// </summary>
public interface ITenantDirectoryService
{
    Task<List<string>> GetActiveTenantIdsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Active tenants paired with the settings needed to (re)build their
    /// per-tenant time-based job triggers - the closing time, club time zone and
    /// reward-reset cadence. Safe to call with no ambient tenant context
    /// (TenantSetting is not tenant-scoped).
    /// </summary>
    Task<List<TenantScheduleInfo>> GetActiveTenantWorkingHoursAsync(CancellationToken cancellationToken);

    /// <summary>
    /// The same scheduling info for one tenant, looked up directly by id. Use this
    /// instead of the tenant-settings cache when you already know the tenant and
    /// need its values regardless of the ambient (HTTP or system) context.
    /// Returns null when the tenant or its settings row is missing.
    /// </summary>
    Task<TenantScheduleInfo?> GetTenantScheduleInfoAsync(string tenantId, CancellationToken cancellationToken);
}

/// <param name="TenantId">The tenant identifier.</param>
/// <param name="EndWorkingHours">The tenant's configured closing time, e.g. "22:00". May be empty for a partially configured tenant.</param>
/// <param name="TimeZoneId">The club's IANA time zone id, e.g. "Europe/Sofia". May be null on rows created before per-tenant time zones existed.</param>
/// <param name="PeriodOfRewardReset">"Weekly" / "Monthly" - drives AddUserChallengePeriodJob.</param>
/// <param name="ResetDayForRewards">Weekday name (weekly) or day-of-month number (monthly).</param>
public record TenantScheduleInfo(
    string TenantId,
    string EndWorkingHours,
    string? TimeZoneId,
    string PeriodOfRewardReset,
    string ResetDayForRewards);
