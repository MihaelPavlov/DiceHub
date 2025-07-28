using DH.Adapter.Scheduling.Jobs;
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
    public SchedulerService(
        ISchedulerFactory schedulerFactory,
        ITenantSettingsCacheService tenantSettingsService,
        ILogger<SchedulerService> logger)
    {
        this.schedulerFactory = schedulerFactory;
        this.tenantSettingsService = tenantSettingsService;
        this.logger = logger;
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

    public async Task ScheduleAddUserPeriodJob()
    {
        try
        {
            var scheduler = await schedulerFactory.GetScheduler();
            var tenantSettings = await tenantSettingsService.GetGlobalTenantSettingsAsync(CancellationToken.None);

            var jobKey = new JobKey(nameof(AddUserChallengePeriodJob));

            if (await scheduler.CheckExists(jobKey))
            {
                await scheduler.DeleteJob(jobKey);
            }

            var triggerKey = new TriggerKey($"WeeklyJobTrigger-{jobKey.Name}");

            Enum.TryParse<TimePeriodType>(tenantSettings.PeriodOfRewardReset, out var timePeriod);

            var runAt = TimePeriodTypeHelper.CalculateNextResetDate(timePeriod, tenantSettings.ResetDayForRewards);

            var job = JobBuilder.Create<AddUserChallengePeriodJob>()
                .WithIdentity(jobKey)
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity(triggerKey)
                .StartAt(runAt)
                .ForJob(jobKey)
                .Build();

            await scheduler.ScheduleJob(job, trigger);

            logger.LogInformation("Scheduled AddUserChallengePeriodJob to run at {RunAt}", runAt);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to schedule AddUserChallengePeriodJob");
        }
    }
}
