using DH.Adapter.Scheduling.Handlers;
using DH.Adapter.Scheduling.Jobs;
using DH.Domain.Adapters.Scheduling;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl.AdoJobStore;

namespace DH.Adapter.Scheduling;

public static class SchedulingDIModule
{
    public static IServiceCollection AddSchedulingAdapter(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IReservationExpirationHandler, ReservationExpirationHandler>();
        services.AddScoped<IUserRewardsExpiryHandler, UserRewardsExpiryHandler>();
        services.AddScoped<IAddUserChallengePeriodHandler, AddUserChallengePeriodHandler>();
        services.AddScoped<IUserRewardsExpirationReminderHandler, UserRewardsExpirationReminderHandler>();
        services.AddScoped<ISchedulerService, SchedulerService>();

        // For testing 
        services.AddTransient<AddUserChallengePeriodJob>();
        services.AddTransient<UserChallengeValidationJob>();

        services.AddQuartz(q =>
        {
            q.UseDefaultThreadPool(tp =>
            {
                tp.MaxConcurrency = 5;
            });
            // Configure the job store for persistence
            q.UsePersistentStore(storeOptions =>
            {
                storeOptions.UseProperties = true;
                storeOptions.UsePostgres(sqlServerOptions =>
                {
                    sqlServerOptions.ConnectionString = configuration.GetConnectionString("DefaultConnection")
                        ?? throw new InvalidConfigurationException("DefaultConnection: Was not found. Place SchedulingDIModule.AddSchedulingAdapter");
                    sqlServerOptions.TablePrefix = "qrtz_";
                });
                storeOptions.PerformSchemaValidation = false;
                storeOptions.UseNewtonsoftJsonSerializer();
                storeOptions.UseClustering(); // Enable clustering if needed
            });

            // Register the job and trigger
            q.AddJob<ExpireReservationJob>(opts => opts.WithIdentity(nameof(ExpireReservationJob))
            .StoreDurably().RequestRecovery());
            q.AddJob<EventChecker>(opts => opts.WithIdentity(nameof(EventChecker))
            .StoreDurably().RequestRecovery());
            // CloseActiveTablesJob, UserChallengeValidationJob, UserChallengeTop3StreakTrackerJob,
            // UserRewardsExpiryJob, UserRewardsExpirationReminderJob and AddUserChallengePeriodJob
            // all fire at tenant-local times, so they are scheduled per tenant (own JobKey/TriggerKey
            // + TenantId JobDataMap) by SchedulerService.ScheduleTenantDailyJobsAsync /
            // ScheduleAddUserPeriodJobForTenant, reconciled on startup by
            // ReconcileTenantDailyJobsAsync - not registered here.

            TriggerDailyJobs(q, services);

            q.AddJobListener<JobListenerForDeadLetterQueue>();
        });

        // Register Quartz.NET hosted service
        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = false);

        return services;
    }

    private static void TriggerDailyJobs(IServiceCollectionQuartzConfigurator service, IServiceCollection services)
    {
        // ExpireReservationJob compares UtcNow against each reservation's exact
        // timestamp, so it just needs to run periodically - the fire time is
        // sweep latency, not a club-local event. It stays a single global cron.
        service.AddTrigger(opts => opts
            .ForJob(nameof(ExpireReservationJob))
            .WithIdentity($"DailyJobTriggers-{nameof(ExpireReservationJob)}")
            .WithCronSchedule("0 20 0 * * ?", cronScheduleBuilder =>
                cronScheduleBuilder.InTimeZone(TimeZoneInfo.Utc)));// Every night 00:20 UTC

        service.AddTrigger(opts => opts
            .ForJob(nameof(EventChecker))
            .WithIdentity($"Every8HoursTrigger-{nameof(EventChecker)}")
            .WithSimpleSchedule(x => x
                .WithIntervalInHours(8)
                .RepeatForever()));

        // .WithCronSchedule("0 0/2 * * * ?")); // Every two mins

        // UserChallengeValidationJob (was 06:00 Sofia), UserChallengeTop3StreakTrackerJob (23:30),
        // UserRewardsExpiryJob (00:00) and UserRewardsExpirationReminderJob (00:10) are now
        // per-tenant: SchedulerService schedules "{JobName}-{tenantId}" triggers at those hours in
        // each tenant's own time zone.
    }
}
