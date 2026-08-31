using DH.Domain.Adapters.Scheduling.Models;

namespace DH.Domain.Adapters.Scheduling;

public interface ISchedulerService
{
    Task<List<ScheduleJobInfo>> GetScheduleJobs();

    /// <summary>
    /// (Re)schedules the per-tenant AddUserChallengePeriodJob for every active tenant, each at its
    /// own next reset date in its own time zone. Existing per-tenant triggers are left untouched.
    /// Kept for the manual "run challenge period job" endpoint.
    /// </summary>
    Task ScheduleAddUserPeriodJob(CancellationToken cancellationToken);

    /// <summary>
    /// (Re)schedules the per-tenant AddUserChallengePeriodJob for one tenant, reading that tenant's
    /// reset cadence / day / time zone from its settings. <paramref name="replaceExisting"/> false
    /// only creates it when missing (startup / tenant setup); true reschedules (settings save, and
    /// the job re-arming itself after a run).
    /// </summary>
    Task ScheduleAddUserPeriodJobForTenant(string tenantId, bool replaceExisting, CancellationToken cancellationToken);

    /// <summary>
    /// As above but with the cadence values supplied directly - used by the job itself, which
    /// already holds the tenant's settings and ambient context. Returns the computed next reset
    /// (UTC), or null when the cadence value can't be parsed.
    /// </summary>
    Task<DateTime?> ScheduleAddUserPeriodJobForTenant(
        string tenantId, string periodOfRewardReset, string resetDayForRewards, string? timeZoneId,
        bool replaceExisting, CancellationToken cancellationToken);

    /// <summary>
    /// (Re)schedules every per-tenant job that fires once a day at a tenant-local
    /// time - CloseActiveTablesJob, UserChallengeValidationJob, UserChallengeTop3StreakTrackerJob,
    /// UserRewardsExpiryJob and UserRewardsExpirationReminderJob - for one explicit tenant, in that
    /// tenant's <paramref name="timeZoneId"/>. Existing triggers are replaced.
    /// Use from flows that have no ambient tenant context (tenant setup, or the
    /// startup reconciler) and from settings updates.
    /// </summary>
    Task ScheduleTenantDailyJobsAsync(string tenantId, string endWorkingHours, string? timeZoneId, CancellationToken cancellationToken);

    /// <summary>
    /// Ensures every active tenant has each per-tenant daily job trigger plus its
    /// AddUserChallengePeriodJob. Existing triggers are left untouched; only missing
    /// ones are created. Also removes the obsolete global triggers those jobs used
    /// before they became per-tenant. Intended to run once at application startup.
    /// </summary>
    Task ReconcileTenantDailyJobsAsync(CancellationToken cancellationToken);
}
