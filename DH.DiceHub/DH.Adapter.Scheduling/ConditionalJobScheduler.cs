using DH.Adapter.Scheduling.Jobs;
using DH.Domain.Enums;
using DH.Domain.Helpers;
using DH.Domain.Services.TenantSettingsService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;

namespace DH.Adapter.Scheduling;

internal class ConditionalJobScheduler : IHostedService
{
    readonly ISchedulerFactory _schedulerFactory;
    readonly IServiceProvider _serviceProvider;
    readonly ILogger<ConditionalJobScheduler> logger;

    public ConditionalJobScheduler(
        ISchedulerFactory schedulerFactory,
        IServiceProvider serviceProvider,
        ILogger<ConditionalJobScheduler> logger)
    {
        _schedulerFactory = schedulerFactory;
        _serviceProvider = serviceProvider;
        this.logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

        using (var scope = _serviceProvider.CreateScope())
        {
            var tenantSettingsCacheService = scope.ServiceProvider.GetRequiredService<ITenantSettingsCacheService>();
            var tenantSettings = await tenantSettingsCacheService.GetGlobalTenantSettingsAsync(cancellationToken);
            if (Enum.TryParse<TimePeriodType>(tenantSettings.PeriodOfRewardReset, out var timePeriod))
            {
                var jobKey = new JobKey(nameof(AddUserChallengePeriodJob));
                var triggerKey = new TriggerKey($"WeeklyJobTrigger-{jobKey.Name}");

                if (!await scheduler.CheckExists(jobKey, cancellationToken))
                {
                    var runAt = TimePeriodTypeHelper.CalculateNextResetDate(timePeriod, tenantSettings.ResetDayForRewards);

                    var job = JobBuilder.Create<AddUserChallengePeriodJob>()
                        .WithIdentity(jobKey)
                        .Build();

                    var trigger = TriggerBuilder.Create()
                        .WithIdentity(triggerKey)
                        .StartAt(runAt)
                        .ForJob(jobKey)
                        .Build();

                    await scheduler.ScheduleJob(job, trigger, cancellationToken);
                    logger.LogInformation("Scheduled job {JobName} with trigger {TriggerName} to run at {RunAt}", jobKey.Name, triggerKey.Name, runAt);
                }
                else
                    logger.LogInformation("Job {JobName} already exists, skipping scheduling.", jobKey.Name);

            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}