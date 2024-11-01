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

            TriggerDailyJobs(q);

            q.AddJobListener<JobListenerForDeadLetterQueue>();
        });

        // Register Quartz.NET hosted service
        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

        return services;
    }

    private static void TriggerDailyJobs(IServiceCollectionQuartzConfigurator service)
    {
        service.AddTrigger(opts => opts
            .ForJob(nameof(UserRewardsExpiryJob))
            .WithIdentity("DailyJobTriggers")
            .WithCronSchedule("0 0 0 * * ?"));  // Cron expression for 00:00
            //.WithCronSchedule("0 * * * * ?"));  // Cron expression for every min
    }
}
