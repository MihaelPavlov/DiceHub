using DH.Domain.Adapters.Scheduling;
using Quartz;

namespace DH.Adapter.Scheduling;

internal class JobManager : IJobManager
{
    readonly ISchedulerFactory schedulerFactory;

    public JobManager(ISchedulerFactory schedulerFactory)
    {
        this.schedulerFactory = schedulerFactory;
    }

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
}
