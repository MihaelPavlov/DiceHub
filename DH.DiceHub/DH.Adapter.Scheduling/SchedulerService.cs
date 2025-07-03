using DH.Adapter.Scheduling.Jobs;
using DH.Domain.Adapters.Scheduling;
using DH.Domain.Enums;
using DH.Domain.Helpers;
using DH.Domain.Services.TenantSettingsService;
using Quartz;

namespace DH.Adapter.Scheduling;

internal class SchedulerService : ISchedulerService
{
    private readonly ISchedulerFactory schedulerFactory;
    private readonly ITenantSettingsCacheService tenantSettingsService;

    public SchedulerService(
        ISchedulerFactory schedulerFactory,
        ITenantSettingsCacheService tenantSettingsService)
    {
        this.schedulerFactory = schedulerFactory;
        this.tenantSettingsService = tenantSettingsService;
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
        }
        catch (Exception EX)
        {
        }
    }
}
