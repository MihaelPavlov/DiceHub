using DH.Adapter.Scheduling.Jobs;
using DH.Domain.Adapters.Scheduling;
using Quartz;

namespace DH.Adapter.Scheduling;

/// <inheritdoc/>
internal class JobManager : IJobManager
{
    readonly ISchedulerFactory schedulerFactory;

    public JobManager(ISchedulerFactory schedulerFactory)
    {
        this.schedulerFactory = schedulerFactory;
    }

    /// <inheritdoc/>
    public async Task CreateReservationJob(int reservationId, DateTime reservationTime, int durationInMinutes)
    {
        var scheduler = await this.schedulerFactory.GetScheduler();

        var job = JobBuilder.Create<ExpireReservationJob>()
            .WithIdentity($"ExpireReservationJob-{reservationId}", "ReservationJobs")
            .UsingJobData("ReservationId", reservationId.ToString())
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity($"ExpireReservationTrigger-{reservationId}", "ReservationJobs")
            .StartAt(reservationTime.AddMinutes(durationInMinutes))
            .Build();

        await scheduler.ScheduleJob(job, trigger);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteJob(string jobName, string jobGroup = "DEFAULT")
    {
        var scheduler = await this.schedulerFactory.GetScheduler();

        var jobKey = new JobKey(jobName, jobGroup);

        return await scheduler.DeleteJob(jobKey);
    }
}
