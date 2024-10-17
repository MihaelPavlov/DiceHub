using DH.Domain.Adapters.Scheduling;
using DH.Domain.Exceptions;
using Quartz;

namespace DH.Adapter.Scheduling.Jobs;

/// <summary>
/// A job responsible for handling the expiration of game reservations. 
/// It uses the <see cref="IReservationExpirationHandler"/> to process the reservation expiration logic.
/// </summary>
internal class ExpireReservationJob : IJob
{
    readonly IReservationExpirationHandler expireReservationJob;
    readonly ISchedulerFactory schedulerFactory;

    public ExpireReservationJob(IReservationExpirationHandler expireReservationJob, ISchedulerFactory schedulerFactory)
    {
        this.expireReservationJob = expireReservationJob;
        this.schedulerFactory = schedulerFactory;
    }

    /// <summary>
    /// Executes the job to process the expiration of a game reservation.
    /// </summary>
    /// <param name="context">The job execution context that contains job details such as reservation ID.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InfrastructureException">Thrown if the reservation expiration process fails.</exception>
    public async Task Execute(IJobExecutionContext context)
    {
        var reservationId = context.JobDetail.JobDataMap.GetInt("ReservationId");

        var isJobSuccessfully = await expireReservationJob.ProcessReservationExpirationAsync(reservationId, CancellationToken.None);

        if (!isJobSuccessfully)
            throw new InfrastructureException("Failed Job");
    }
}
