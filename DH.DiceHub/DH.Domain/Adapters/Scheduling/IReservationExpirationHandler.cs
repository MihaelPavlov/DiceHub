namespace DH.Domain.Adapters.Scheduling;

/// <summary>
/// Handles the processing of game and table reservation expirations, including managing failed expirations.
/// </summary>
public interface IReservationExpirationHandler
{
    /// <summary>
    /// Processes the expiration of a game and table reservations which was forgot 24/h cycle.
    /// If the reservation has expired, updates the related inventory and reservation status.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for task cancellation.</param>
    Task ProcessReservationExpirationAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Handles any failures in processing a reservation expiration by logging the failure details.
    /// </summary>
    /// <param name="data">The failed reservation data.</param>
    /// <param name="errorMessage">The error message explaining the failure.</param>
    /// <param name="cancellationToken">Cancellation token for task cancellation.</param>
    Task ProcessFailedReservationExpirationAsync(string data, string errorMessage, CancellationToken cancellationToken);
}