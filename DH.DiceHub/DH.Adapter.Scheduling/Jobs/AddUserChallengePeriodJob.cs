using DH.Domain.Adapters.Scheduling;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Helpers;
using DH.Domain.Services.TenantSettingsService;
using DH.OperationResultCore.Exceptions;
using Quartz;

namespace DH.Adapter.Scheduling.Jobs;

public class AddUserChallengePeriodJob : IJob
{
    readonly IAddUserChallengePeriodHandler addUserChallengePeriodHandler;
    private readonly ISchedulerFactory schedulerFactory;
    private readonly ITenantSettingsCacheService tenantSettingsService;

    public AddUserChallengePeriodJob(
        IAddUserChallengePeriodHandler addUserChallengePeriodHandler,
        ISchedulerFactory schedulerFactory,
        ITenantSettingsCacheService tenantSettingsService)
    {
        this.addUserChallengePeriodHandler = addUserChallengePeriodHandler;
        this.schedulerFactory = schedulerFactory;
        this.tenantSettingsService = tenantSettingsService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            await this.addUserChallengePeriodHandler.InitializeNewPeriods(CancellationToken.None);

            var scheduler = await this.schedulerFactory.GetScheduler();
            var tenantSettings = await this.tenantSettingsService.GetGlobalTenantSettingsAsync(CancellationToken.None);

            var jobKey = new JobKey(nameof(AddUserChallengePeriodJob));

            if (await scheduler.CheckExists(jobKey))
            {
                await scheduler.DeleteJob(jobKey);
            }

            var triggerKey = new TriggerKey($"WeeklyJobTrigger-{jobKey.Name}");

            Enum.TryParse<TimePeriodType>(tenantSettings.PeriodOfRewardReset, out var timePeriod);

            var runAt = TimePeriodTypeHelper.CalculateNextResetDate(timePeriod, tenantSettings.ResetDayForRewards).AddDays(7); // use the same logic as above

            var nextTrigger = TriggerBuilder.Create()
                .ForJob(jobKey)
                .WithIdentity(triggerKey)
                .StartAt(runAt)
                .Build();

            var job = JobBuilder.Create<AddUserChallengePeriodJob>()
                 .WithIdentity(jobKey)
                 .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity(triggerKey)
                .StartAt(runAt)
                .ForJob(jobKey)
                .Build();

            await scheduler.ScheduleJob(job, trigger, CancellationToken.None);
        }
        catch (Exception ex)
        {
            throw new InfrastructureException(ex.Message);
        }
    }
}
