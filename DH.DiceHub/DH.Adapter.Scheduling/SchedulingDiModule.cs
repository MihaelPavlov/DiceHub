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
                storeOptions.UseNewtonsoftJsonSerializer();
                storeOptions.UseClustering(); // Enable clustering if needed
            });

            // Register the job and trigger
            q.AddJob<ExpireReservationJob>(opts => opts.WithIdentity("ExpireReservationJob").StoreDurably());

            q.AddJobListener<JobListenerForDeadLetterQueue>();
            //q.UseJobListener<JobListenerForDeadLetterQueue>();
        });

        // Register Quartz.NET hosted service
        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

        return services;
    }
}
