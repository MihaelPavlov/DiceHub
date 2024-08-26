using DH.Domain.Adapters.Scheduling;
using DH.Domain.Exceptions;
using Quartz;

namespace DH.Adapter.Scheduling;

internal class ExpireReservationJob : IJob
{
    readonly IReservationExpirationHandler expireReservationJob;
    readonly ISchedulerFactory schedulerFactory;

    public ExpireReservationJob(IReservationExpirationHandler expireReservationJob, ISchedulerFactory schedulerFactory)
    {
        this.expireReservationJob = expireReservationJob;
        this.schedulerFactory = schedulerFactory;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var reservationId = context.JobDetail.JobDataMap.GetInt("ReservationId");

        var isJobSuccessfully = await this.expireReservationJob.ProcessReservationExpirationAsync(reservationId, CancellationToken.None);

        if (!isJobSuccessfully)
            throw new InfrastructureException("Failed Job");
    }

}
