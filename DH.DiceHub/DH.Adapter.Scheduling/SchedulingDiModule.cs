using DH.Adapter.Scheduling.Handlers;
using DH.Adapter.Scheduling.Jobs;
using DH.Domain.Adapters.Scheduling;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace DH.Adapter.Scheduling;

public static class SchedulingDIModule
{
    public static IServiceCollection AddSchedulingAdapter(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IJobManager, JobManager>();
        services.AddScoped<IReservationExpirationHandler, ReservationExpirationHandler>();
        services.AddScoped<IUserRewardsExpiryHandler, UserRewardsExpiryHandler>();
        services.AddScoped<IAddUserChallengePeriodHandler, AddUserChallengePeriodHandler>();
        services.AddScoped<IUserRewardsExpirationReminderHandler, UserRewardsExpirationReminderHandler>();

        services.AddQuartz(q =>
        {
            // Configure the job store for persistence
            q.UsePersistentStore(storeOptions =>
            {
                storeOptions.UseProperties = true;
                storeOptions.UseSqlServer(sqlServerOptions =>
                {
                    sqlServerOptions.ConnectionString = configuration.GetConnectionString("DefaultConnection");
                    sqlServerOptions.TablePrefix = "QRTZ_";
                });
                storeOptions.PerformSchemaValidation = false;
                storeOptions.UseNewtonsoftJsonSerializer();
                storeOptions.UseClustering(); // Enable clustering if needed
            });

            // Register the job and trigger
            q.AddJob<ExpireReservationJob>(opts => opts.WithIdentity(nameof(ExpireReservationJob)).StoreDurably());

            q.AddJob<UserRewardsExpiryJob>(opts => opts.WithIdentity(nameof(UserRewardsExpiryJob)));
            q.AddJob<UserRewardsExpirationReminderJob>(opts => opts.WithIdentity(nameof(UserRewardsExpirationReminderJob)));
            q.AddJob<AddUserChallengePeriodJob>(opts => opts.WithIdentity(nameof(AddUserChallengePeriodJob)).StoreDurably());

            TriggerDailyJobs(q, services);

            q.AddJobListener<JobListenerForDeadLetterQueue>();
        });

        // Register Quartz.NET hosted service
        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

        services.AddHostedService<ConditionalJobScheduler>();

        return services;
    }

    private static void TriggerDailyJobs(IServiceCollectionQuartzConfigurator service, IServiceCollection services)
    {
        service.AddTrigger(opts => opts
            .ForJob(nameof(UserRewardsExpiryJob))
            .WithIdentity($"DailyJobTriggers-{nameof(UserRewardsExpiryJob)}")
            .WithCronSchedule("0 0 0 * * ?"));// Every night 00:00 UTC 

        service.AddTrigger(opts => opts
           .ForJob(nameof(UserRewardsExpirationReminderJob))
           .WithIdentity($"DailyJobTriggers-{nameof(UserRewardsExpirationReminderJob)}")
           .WithCronSchedule("0 0/2 * * * ?")); // Every two mins
    }
}
