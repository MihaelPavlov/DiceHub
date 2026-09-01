using DH.Adapter.Scheduling.Jobs;
using DH.Domain.Adapters.Scheduling;
using DH.Domain.Adapters.Scheduling.Models;
using DH.Domain.Enums;
using DH.Domain.Helpers;
using DH.Domain.Services;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl.Matchers;

namespace DH.Adapter.Scheduling;

internal class SchedulerService : ISchedulerService
{
    private readonly ISchedulerFactory schedulerFactory;
    private readonly ITenantDirectoryService tenantDirectoryService;
    private readonly ILogger<SchedulerService> logger;

    public SchedulerService(
        ISchedulerFactory schedulerFactory,
        ITenantDirectoryService tenantDirectoryService,
        ILogger<SchedulerService> logger)
    {
        this.schedulerFactory = schedulerFactory;
        this.tenantDirectoryService = tenantDirectoryService;
        this.logger = logger;
    }

    /// <summary>
    /// Every job that must fire once a day at a tenant-local wall-clock time. Each
    /// gets its own Quartz job + trigger per tenant (key "{Name}-{tenantId}",
    /// TenantId in the JobDataMap) so the hour is interpreted in that tenant's
    /// time zone. <see cref="DailyTenantJobSpec.ResolveTime"/> returns null when a
    /// fire time can't be derived (e.g. blank working hours), which skips that job.
    /// </summary>
    private static readonly DailyTenantJobSpec[] DailyTenantJobSpecs =
    [
        new(typeof(CloseActiveTablesJob), nameof(CloseActiveTablesJob), endWorkingHours =>
            TimeOnly.TryParse(endWorkingHours, out var end)
                ? (end.AddMinutes(10).Hour, end.AddMinutes(10).Minute)
                : null),
        new(typeof(UserChallengeValidationJob), nameof(UserChallengeValidationJob), _ => (6, 0)),
        new(typeof(UserChallengeTop3StreakTrackerJob), nameof(UserChallengeTop3StreakTrackerJob), _ => (23, 30)),
        // Reward sweeps run just after local midnight, staggered like the old global crons.
        new(typeof(UserRewardsExpiryJob), nameof(UserRewardsExpiryJob), _ => (0, 0)),
        new(typeof(UserRewardsExpirationReminderJob), nameof(UserRewardsExpirationReminderJob), _ => (0, 10)),
    ];

    private sealed record DailyTenantJobSpec(
        Type JobType,
        string Name,
        Func<string, (int Hour, int Minute)?> ResolveTime);

    public async Task<List<ScheduleJobInfo>> GetScheduleJobs()
    {
        var scheduler = await schedulerFactory.GetScheduler();

        var jobGroups = await scheduler.GetJobGroupNames();
        var result = new List<ScheduleJobInfo>();
        foreach (var group in jobGroups)
        {
            var jobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(group));

            foreach (var jobKey in jobKeys)
            {
                var triggers = await scheduler.GetTriggersOfJob(jobKey);

                foreach (var trigger in triggers)
                {
                    result.Add(new ScheduleJobInfo
                    {
                        JobKeyName = jobKey.Name,
                        TriggerKeyName = trigger.Key.Name,
                        NextFireTime = trigger.GetNextFireTimeUtc()?.DateTime,
                        PreviousFireTime = trigger.GetPreviousFireTimeUtc()?.DateTime
                    });
                }
            }
        }

        return result;
    }

    public async Task ScheduleAddUserPeriodJob(CancellationToken cancellationToken)
    {
        List<string> tenantIds;
        try
        {
            tenantIds = await this.tenantDirectoryService.GetActiveTenantIdsAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to enumerate tenants while scheduling AddUserChallengePeriodJob.");
            return;
        }

        foreach (var tenantId in tenantIds)
            await ScheduleAddUserPeriodJobForTenant(tenantId, replaceExisting: false, cancellationToken);
    }

    public async Task ScheduleAddUserPeriodJobForTenant(string tenantId, bool replaceExisting, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(tenantId))
            throw new InvalidOperationException($"{nameof(ScheduleAddUserPeriodJobForTenant)} requires a tenant id.");

        try
        {
            // Read the tenant's cadence directly by id - NOT via the tenant-settings
            // cache. That cache resolves "which tenant" through IUserContextFactory,
            // which during an HTTP request (tenant setup / settings save) ignores the
            // ambient RunAsTenantAsync context and falls back to the orphan global
            // TenantSetting (Id == 1), so a non-default tenant would get the wrong
            // time zone / reset day.
            var info = await this.tenantDirectoryService.GetTenantScheduleInfoAsync(tenantId, cancellationToken);
            if (info is null)
            {
                this.logger.LogWarning(
                    "Skipped scheduling {Job} for tenant {TenantId}: no settings row found.",
                    nameof(AddUserChallengePeriodJob), tenantId);
                return;
            }

            await ScheduleAddUserPeriodJobForTenant(
                tenantId, info.PeriodOfRewardReset, info.ResetDayForRewards, info.TimeZoneId, replaceExisting, cancellationToken);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to schedule AddUserChallengePeriodJob for tenant {TenantId}.", tenantId);
        }
    }

    public async Task<DateTime?> ScheduleAddUserPeriodJobForTenant(
        string tenantId, string periodOfRewardReset, string resetDayForRewards, string? timeZoneId,
        bool replaceExisting, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(tenantId))
            throw new InvalidOperationException($"{nameof(ScheduleAddUserPeriodJobForTenant)} requires a tenant id.");

        if (!Enum.TryParse<TimePeriodType>(periodOfRewardReset, out var timePeriod))
        {
            this.logger.LogWarning(
                "Skipped scheduling {Job} for tenant {TenantId}: PeriodOfRewardReset '{Period}' is not valid.",
                nameof(AddUserChallengePeriodJob), tenantId, periodOfRewardReset);
            return null;
        }

        var jobKey = new JobKey($"{nameof(AddUserChallengePeriodJob)}-{tenantId}");
        var triggerKey = new TriggerKey($"WeeklyJobTrigger-{nameof(AddUserChallengePeriodJob)}-{tenantId}");

        var scheduler = await this.schedulerFactory.GetScheduler(cancellationToken);

        var alreadyExists = await scheduler.CheckExists(jobKey, cancellationToken);
        if (alreadyExists && !replaceExisting)
            return null;

        var runAt = TimePeriodTypeHelper.CalculateNextResetDate(timePeriod, resetDayForRewards, timeZoneId);

        if (alreadyExists)
        {
            await scheduler.UnscheduleJob(triggerKey, cancellationToken);
            await scheduler.DeleteJob(jobKey, cancellationToken);
        }

        var job = JobBuilder.Create<AddUserChallengePeriodJob>()
            .WithIdentity(jobKey)
            .UsingJobData("TenantId", tenantId)
            // Durable so the job survives its one-shot trigger completing; it re-arms
            // itself from Execute (delete + recreate via replaceExisting).
            .StoreDurably()
            .RequestRecovery()
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity(triggerKey)
            .StartAt(runAt)
            .ForJob(jobKey)
            .Build();

        await scheduler.ScheduleJob(job, trigger, cancellationToken);

        this.logger.LogInformation(
            "Scheduled {Job} for tenant {TenantId} to run at {RunAt:o}.",
            nameof(AddUserChallengePeriodJob), tenantId, runAt);

        return runAt;
    }

    public async Task ScheduleTenantDailyJobsAsync(string tenantId, string endWorkingHours, string? timeZoneId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(tenantId))
            throw new InvalidOperationException($"{nameof(ScheduleTenantDailyJobsAsync)} requires a tenant id.");

        foreach (var spec in DailyTenantJobSpecs)
            await ScheduleDailyTenantJobCore(spec, tenantId, endWorkingHours, timeZoneId, replaceExisting: true, cancellationToken);
    }

    public async Task ReconcileTenantDailyJobsAsync(CancellationToken cancellationToken)
    {
        await ResetErroredTriggersAsync(cancellationToken);

        await PurgeObsoleteGlobalDailySchedulesAsync(cancellationToken);

        List<TenantScheduleInfo> tenants;
        try
        {
            tenants = await this.tenantDirectoryService.GetActiveTenantWorkingHoursAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to enumerate tenants while reconciling per-tenant daily job triggers.");
            return;
        }

        foreach (var tenant in tenants)
        {
            foreach (var spec in DailyTenantJobSpecs)
            {
                try
                {
                    // replaceExisting:false - startup back-fill only. Tenants that already
                    // have a trigger keep it (and its next-fire-time) untouched; tenant
                    // setup and settings saves refresh it via ScheduleTenantDailyJobsAsync.
                    await ScheduleDailyTenantJobCore(
                        spec, tenant.TenantId, tenant.EndWorkingHours, tenant.TimeZoneId,
                        replaceExisting: false, cancellationToken);
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, "Failed to reconcile {Job} trigger for tenant {TenantId}.", spec.Name, tenant.TenantId);
                }
            }

            // AddUserChallengePeriodJob is a one-shot self-rescheduling trigger rather
            // than a daily cron, so it lives outside DailyTenantJobSpecs.
            // replaceExisting:true here (unlike the daily crons): its fire time is a
            // pure function of the tenant's reset cadence / day / time zone, so
            // recomputing it on every boot is idempotent and self-heals any drift.
            try
            {
                await ScheduleAddUserPeriodJobForTenant(
                    tenant.TenantId, tenant.PeriodOfRewardReset, tenant.ResetDayForRewards, tenant.TimeZoneId,
                    replaceExisting: true, cancellationToken);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to reconcile {Job} trigger for tenant {TenantId}.", nameof(AddUserChallengePeriodJob), tenant.TenantId);
            }
        }
    }

    private async Task ScheduleDailyTenantJobCore(
        DailyTenantJobSpec spec,
        string tenantId,
        string endWorkingHours,
        string? timeZoneId,
        bool replaceExisting,
        CancellationToken cancellationToken)
    {
        var jobKey = new JobKey($"{spec.Name}-{tenantId}");
        var triggerKey = new TriggerKey($"DailyTrigger-{spec.Name}-{tenantId}");

        var scheduler = await this.schedulerFactory.GetScheduler(cancellationToken);

        var alreadyExists = await scheduler.CheckExists(jobKey, cancellationToken);
        if (alreadyExists && !replaceExisting)
            return;

        var time = spec.ResolveTime(endWorkingHours);
        if (time is null)
        {
            this.logger.LogWarning(
                "Skipped scheduling {Job} for tenant {TenantId}: could not derive a fire time (EndWorkingHours '{EndWorkingHours}').",
                spec.Name, tenantId, endWorkingHours);
            return;
        }

        var (hour, minute) = time.Value;

        if (!TimeZoneResolver.TryResolve(timeZoneId, out var timeZone))
            this.logger.LogWarning(
                "Tenant {TenantId} has an unresolved time zone '{TimeZone}'; using {Default} for {Job}.",
                tenantId, timeZoneId, TimeZoneResolver.DefaultTimeZoneId, spec.Name);

        if (alreadyExists)
        {
            await scheduler.UnscheduleJob(triggerKey, cancellationToken);
            await scheduler.DeleteJob(jobKey, cancellationToken);
        }

        var job = JobBuilder.Create(spec.JobType)
            .WithIdentity(jobKey)
            .UsingJobData("TenantId", tenantId)
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity(triggerKey)
            .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(hour, minute)
                .InTimeZone(timeZone))
            .Build();

        await scheduler.ScheduleJob(job, trigger, cancellationToken);

        this.logger.LogInformation(
            "Scheduled {Job} for tenant {TenantId} at {Hour:D2}:{Minute:D2} ({TimeZone}).",
            spec.Name, tenantId, hour, minute, timeZone.Id);
    }

    /// <summary>
    /// Any trigger whose job threw is parked in ERROR state by Quartz and never fires
    /// again. That is invisible until someone notices a whole feature has silently
    /// stopped (per-tenant reward periods froze this way after the jobs went
    /// per-tenant). Reset every ERROR trigger back to WAITING on startup so a single
    /// transient failure can't permanently disable a schedule.
    /// </summary>
    private async Task ResetErroredTriggersAsync(CancellationToken cancellationToken)
    {
        try
        {
            var scheduler = await this.schedulerFactory.GetScheduler(cancellationToken);
            var triggerKeys = await scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup(), cancellationToken);

            foreach (var triggerKey in triggerKeys)
            {
                try
                {
                    if (await scheduler.GetTriggerState(triggerKey, cancellationToken) != TriggerState.Error)
                        continue;

                    await scheduler.ResetTriggerFromErrorState(triggerKey, cancellationToken);
                    this.logger.LogWarning("Reset trigger {Trigger} from ERROR state back to WAITING.", triggerKey);
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, "Failed to reset trigger {Trigger} from ERROR state.", triggerKey);
                }
            }
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to enumerate triggers while resetting ERROR-state schedules.");
        }
    }

    /// <summary>
    /// UserChallengeValidationJob, UserChallengeTop3StreakTrackerJob, UserRewardsExpiryJob,
    /// UserRewardsExpirationReminderJob and AddUserChallengePeriodJob all used to run from a single
    /// global trigger before they became per-tenant. Quartz's persistent store keeps those old
    /// job/trigger rows until they are explicitly removed, so without this a stale global trigger
    /// would keep firing the all-tenants code path alongside the new per-tenant triggers.
    /// </summary>
    private async Task PurgeObsoleteGlobalDailySchedulesAsync(CancellationToken cancellationToken)
    {
        var scheduler = await this.schedulerFactory.GetScheduler(cancellationToken);

        (string JobName, string TriggerName)[] obsolete =
        [
            (nameof(UserChallengeValidationJob), $"DailyJobTriggers-{nameof(UserChallengeValidationJob)}"),
            (nameof(UserChallengeTop3StreakTrackerJob), $"DailyJobTriggers-{nameof(UserChallengeTop3StreakTrackerJob)}"),
            (nameof(UserRewardsExpiryJob), $"DailyJobTriggers-{nameof(UserRewardsExpiryJob)}"),
            (nameof(UserRewardsExpirationReminderJob), $"DailyJobTriggers-{nameof(UserRewardsExpirationReminderJob)}"),
            (nameof(AddUserChallengePeriodJob), $"WeeklyJobTrigger-{nameof(AddUserChallengePeriodJob)}"),
            // Bare durable job left over from before CloseActiveTablesJob was per-tenant;
            // it has no trigger and never fires. No obsolete trigger name to match - the
            // "delete bare job with 0 triggers" branch below removes it.
            (nameof(CloseActiveTablesJob), $"DailyTrigger-{nameof(CloseActiveTablesJob)}"),
        ];

        foreach (var (jobName, triggerName) in obsolete)
        {
            try
            {
                var triggerKey = new TriggerKey(triggerName);
                if (await scheduler.CheckExists(triggerKey, cancellationToken))
                {
                    await scheduler.UnscheduleJob(triggerKey, cancellationToken);
                    this.logger.LogInformation("Removed obsolete global trigger {Trigger}.", triggerName);
                }

                // The bare-named durable job (no "-{tenantId}" suffix) is now unused.
                var jobKey = new JobKey(jobName);
                if (await scheduler.CheckExists(jobKey, cancellationToken)
                    && (await scheduler.GetTriggersOfJob(jobKey, cancellationToken)).Count == 0)
                {
                    await scheduler.DeleteJob(jobKey, cancellationToken);
                    this.logger.LogInformation("Removed obsolete global job {Job}.", jobName);
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to purge obsolete global schedule for {Job}.", jobName);
            }
        }
    }
}
