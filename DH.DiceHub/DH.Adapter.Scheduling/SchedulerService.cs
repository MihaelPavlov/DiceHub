using DH.Adapter.Scheduling.Jobs;
using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Scheduling;
using DH.Domain.Adapters.Scheduling.Models;
using DH.Domain.Enums;
using DH.Domain.Helpers;
using DH.Domain.Services.TenantSettingsService;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl.Matchers;

namespace DH.Adapter.Scheduling;

internal class SchedulerService : ISchedulerService
{
    private readonly ISchedulerFactory schedulerFactory;
    private readonly ITenantSettingsCacheService tenantSettingsService;
    private readonly ILogger<SchedulerService> logger;
    private readonly IUserContext userContext;

    public SchedulerService(
        ISchedulerFactory schedulerFactory,
        ITenantSettingsCacheService tenantSettingsService,
        ILogger<SchedulerService> logger,
        IUserContext userContext)
    {
        this.schedulerFactory = schedulerFactory;
        this.tenantSettingsService = tenantSettingsService;
        this.logger = logger;
        this.userContext = userContext;
    }

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

    public async Task<bool> DoesAddUserChallengePeriodJobExists()
    {

        try
        {
            var scheduler = await schedulerFactory.GetScheduler();
            var tenantSettings = await tenantSettingsService.GetGlobalTenantSettingsAsync(CancellationToken.None);

            var jobKey = new JobKey(nameof(AddUserChallengePeriodJob));

            return await scheduler.CheckExists(jobKey);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to check if AddUserChallengePeriodJob exists");
        }
        return false;
    }

    public async Task ScheduleAddUserPeriodJob(CancellationToken cancellationToken)
    {
        try
        {
            var scheduler = await schedulerFactory.GetScheduler(cancellationToken);

            var tenantSettings = await tenantSettingsService.GetGlobalTenantSettingsAsync(cancellationToken);
            if (Enum.TryParse<TimePeriodType>(tenantSettings.PeriodOfRewardReset, out var timePeriod))
            {
                var jobKey = new JobKey(nameof(AddUserChallengePeriodJob));

                var triggers = await scheduler.GetTriggersOfJob(jobKey, cancellationToken);
                bool hasTrigger = triggers.Any(t => t is ICronTrigger || t is ISimpleTrigger);

                if (!hasTrigger)
                {
                    var runAt = TimePeriodTypeHelper.CalculateNextResetDate(timePeriod, tenantSettings.ResetDayForRewards);
                    var offset = TimeZoneHelper.GetOffsetForTimeZone(runAt, "Europe/Sofia");
                    runAt = runAt.AddHours(-offset?.TotalHours ?? 0);

                    //var job = await scheduler.GetJobDetail(jobKey, cancellationToken)
                    //          ?? JobBuilder.Create<AddUserChallengePeriodJob>()
                    //                       .WithIdentity(jobKey)
                    //                       .Build();

                    var triggerKey = new TriggerKey($"WeeklyJobTrigger-{jobKey.Name}");
                    var trigger = TriggerBuilder.Create()
                                                .WithIdentity(triggerKey)
                                                .StartAt(runAt)
                                                .ForJob(jobKey)
                                                .Build();

                    await scheduler.ScheduleJob(trigger, cancellationToken);
                    logger.LogInformation("Scheduled job {JobName} with trigger {TriggerName} to run at {RunAt}", jobKey.Name, triggerKey.Name, runAt);
                }
                else
                {
                    logger.LogInformation("Job {JobName} already has a trigger, skipping scheduling.", jobKey.Name);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to schedule AddUserChallengePeriodJob");
        }
    }

    public async Task ScheduleCloseActiveTablesJob(CancellationToken cancellationToken)
    {
        var scheduler = await this.schedulerFactory.GetScheduler(cancellationToken);
        var tenantSettings = await this.tenantSettingsService.GetGlobalTenantSettingsAsync(cancellationToken);

        var endTime = TimeOnly.Parse(tenantSettings.EndWorkingHours)
               .AddMinutes(10);

        var (hour, minute) = (endTime.Hour, endTime.Minute);

        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(this.userContext.TimeZone ?? "Europe/Sofia");

        var jobKey = new JobKey(nameof(CloseActiveTablesJob));
        var triggerKey = new TriggerKey($"DailyTrigger-{nameof(CloseActiveTablesJob)}");

        // Check if job already exists
        if (await scheduler.CheckExists(jobKey, cancellationToken))
        {
            // Unschedule the old trigger and delete old job
            await scheduler.UnscheduleJob(triggerKey, cancellationToken);
            await scheduler.DeleteJob(jobKey, cancellationToken);
        }

        var job = JobBuilder.Create<CloseActiveTablesJob>()
            .WithIdentity(jobKey)
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity(triggerKey)
            .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(hour, minute)
                .InTimeZone(timeZone))
            .Build();

        await scheduler.ScheduleJob(job, trigger, cancellationToken);
    }
}
