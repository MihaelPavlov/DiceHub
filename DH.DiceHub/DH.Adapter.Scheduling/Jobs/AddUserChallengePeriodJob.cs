using DH.Domain.Adapters.Scheduling;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Helpers;
using DH.Domain.Repositories;
using DH.Domain.Services.TenantSettingsService;
using DH.OperationResultCore.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Threading;

namespace DH.Adapter.Scheduling.Jobs;

// The job responsible to create the new Challenge Period Performance for all users
// And rescheduled the next one
public class AddUserChallengePeriodJob : IJob
{
    readonly IAddUserChallengePeriodHandler addUserChallengePeriodHandler;
    readonly ISchedulerFactory schedulerFactory;
    readonly ITenantSettingsCacheService tenantSettingsService;
    readonly ILogger<AddUserChallengePeriodJob> logger;
    readonly IRepository<TenantSetting> repository;

    static readonly string JobName = nameof(AddUserChallengePeriodJob);
    static readonly string TriggerName = $"WeeklyJobTrigger-{JobName}";

    public AddUserChallengePeriodJob(
        IAddUserChallengePeriodHandler addUserChallengePeriodHandler,
        ISchedulerFactory schedulerFactory,
        ITenantSettingsCacheService tenantSettingsService,
        IRepository<TenantSetting> repository,
        ILogger<AddUserChallengePeriodJob> logger)
    {
        this.addUserChallengePeriodHandler = addUserChallengePeriodHandler;
        this.schedulerFactory = schedulerFactory;
        this.tenantSettingsService = tenantSettingsService;
        this.repository = repository;
        this.logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            this.logger.LogInformation("AddUserChallengePeriodJob started execution at {RunAt}.", DateTime.UtcNow);

            await this.addUserChallengePeriodHandler.InitializeNewPeriods(CancellationToken.None);

            var scheduler = await this.schedulerFactory.GetScheduler();
            var tenantSettings = await this.tenantSettingsService.GetGlobalTenantSettingsAsync(CancellationToken.None);

            Enum.TryParse<TimePeriodType>(tenantSettings.PeriodOfRewardReset, out var timePeriod);

            var nextRunAt = TimePeriodTypeHelper.CalculateNextResetDate(timePeriod, tenantSettings.ResetDayForRewards);

            //var offset = TimeZoneHelper.GetOffsetForTimeZone(runAt, "Europe/Sofia");
            //runAt = runAt.AddHours(-offset?.TotalHours ?? 0);
            this.logger.LogInformation("AddUserChallengePeriodJob will be rescheduled to run at {NextRunAt}.", nextRunAt);

            var jobKey = new JobKey(JobName);
            var triggerKey = new TriggerKey(TriggerName);
            // Ensure job exists (create only once)
            if (!await scheduler.CheckExists(jobKey))
            {
                var job = JobBuilder.Create<AddUserChallengePeriodJob>()
                    .WithIdentity(jobKey)
                    .StoreDurably()
                    .Build();

                await scheduler.AddJob(job, true, CancellationToken.None);
            }

            // Reschedule trigger
            var trigger = TriggerBuilder.Create()
                .WithIdentity(triggerKey)
                .ForJob(jobKey)
                .StartAt(nextRunAt)
                .Build();

            await scheduler.RescheduleJob(triggerKey, trigger);

            this.logger.LogInformation("AddUserChallengePeriodJob successfully rescheduled for {NextRunAt}.", nextRunAt);

            var dbSettings = await this.repository.GetByAsyncWithTracking(x => x.Id == tenantSettings.Id, CancellationToken.None);
            if (dbSettings != null)
            {
                dbSettings.NextResetTimeOfPeriod = nextRunAt.ToUniversalTime();
                await this.repository.SaveChangesAsync(CancellationToken.None);
            }
        }
        catch (Exception ex)
        {
            throw new InfrastructureException(ex.Message);
        }
    }
}
