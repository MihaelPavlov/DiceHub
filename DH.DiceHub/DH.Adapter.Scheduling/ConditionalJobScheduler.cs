using DH.Adapter.Scheduling.Jobs;
using DH.Domain.Enums;
using DH.Domain.Services.TenantSettingsService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;

namespace DH.Adapter.Scheduling;

internal class ConditionalJobScheduler : IHostedService
{
    readonly ISchedulerFactory _schedulerFactory;
    readonly IServiceProvider _serviceProvider;

    public ConditionalJobScheduler(ISchedulerFactory schedulerFactory, IServiceProvider serviceProvider)
    {
        _schedulerFactory = schedulerFactory;
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

        // Resolve IJobConditionService to check the condition
        using (var scope = _serviceProvider.CreateScope())
        {
            var tenantSettingsCacheService = scope.ServiceProvider.GetRequiredService<ITenantSettingsCacheService>();
            var tenantSettings = await tenantSettingsCacheService.GetGlobalTenantSettingsAsync(cancellationToken);
            if (Enum.TryParse<TimePeriodType>(tenantSettings.PeriodOfRewardReset, out var timePeriod))
            {

                if (timePeriod == TimePeriodType.Weekly)
                {
                    string GetCronDay(WeekDays day) => day switch
                    {
                        WeekDays.Sunday => "SUN",
                        WeekDays.Monday => "MON",
                        WeekDays.Tuesday => "TUE",
                        WeekDays.Wednesday => "WED",
                        WeekDays.Thursday => "THU",
                        WeekDays.Friday => "FRI",
                        WeekDays.Saturday => "SAT",
                        _ => "SUN"
                    };

                    var cronDay = GetCronDay(Enum.Parse<WeekDays>(tenantSettings.ResetDayForRewards));

                    if (!await scheduler.CheckExists(new TriggerKey($"WeeklyJobTrigger-{nameof(AddUserChallengePeriodJob)}", "DEFAULT")))
                    {
                        var weeklyJobTrigger = TriggerBuilder.Create()
                        .ForJob(nameof(AddUserChallengePeriodJob))
                        .WithIdentity($"WeeklyJobTrigger-{nameof(AddUserChallengePeriodJob)}")
                        //.WithCronSchedule("0 */2 * * * ?")  // Cron expression for every 10 minutes
                        .WithCronSchedule($"0 0 0 ? * {cronDay}")
                        .Build();

                        await scheduler.ScheduleJob(weeklyJobTrigger, cancellationToken);
                    }
                }
                else if (timePeriod == TimePeriodType.Monthly)
                {
                    if (!await scheduler.CheckExists(new TriggerKey($"WeeklyJobTrigger-{nameof(AddUserChallengePeriodJob)}", "DEFAULT")))
                    {
                        var monthlyJobTrigger = TriggerBuilder.Create()
                        .ForJob(nameof(AddUserChallengePeriodJob))
                        .WithIdentity($"MonthlyJobTrigger-{nameof(AddUserChallengePeriodJob)}")
                        .WithCronSchedule("0 0 0 L * ?") // Runs at 00:00 on the last day of the month
                        .Build();                        //L: Indicates the last day of the month.

                        await scheduler.ScheduleJob(monthlyJobTrigger, cancellationToken);
                    }
                }
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}