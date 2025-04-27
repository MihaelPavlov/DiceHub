using DH.Domain.Adapters.Scheduling;
using DH.OperationResultCore.Exceptions;
using Quartz;

namespace DH.Adapter.Scheduling.Jobs;

/// <summary>
/// A job responsible for handling the expiration of game and table reservations. 
/// It uses the <see cref="IReservationExpirationHandler"/> to process the reservation expiration logic.
/// </summary>
internal class ExpireReservationJob : IJob
{
    readonly IReservationExpirationHandler expireReservationJob;

    public ExpireReservationJob(IReservationExpirationHandler expireReservationJob)
    {
        this.expireReservationJob = expireReservationJob;
    }

    /// <summary>
    /// Executes the job to process the expiration of a game and table reservation.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InfrastructureException">Thrown if the reservation expiration process fails.</exception>
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            await expireReservationJob.ProcessReservationExpirationAsync(CancellationToken.None);
        }
        catch (Exception ex)
        {
            throw new InfrastructureException(ex.Message);
        }
    }
}
