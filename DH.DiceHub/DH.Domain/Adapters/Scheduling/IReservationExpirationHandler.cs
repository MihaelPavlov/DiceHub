namespace DH.Domain.Adapters.Scheduling;

/// <summary>
/// Handles the processing of game reservation expirations, including managing failed expirations.
/// </summary>
public interface IReservationExpirationHandler
{
    /// <summary>
    /// Processes the expiration of a game reservation based on its ID.
    /// If the reservation has expired, updates the related inventory and reservation status.
    /// </summary>
    /// <param name="reservationId">The ID of the reservation to be processed.</param>
    /// <param name="cancellationToken">Cancellation token for task cancellation.</param>
    /// <returns>Returns a boolean indicating whether the reservation was successfully expired.</returns>
    Task<bool> ProcessReservationExpirationAsync(int reservationId, CancellationToken cancellationToken);

    /// <summary>
    /// Handles any failures in processing a reservation expiration by logging the failure details.
    /// </summary>
    /// <param name="data">The failed reservation data.</param>
    /// <param name="errorMessage">The error message explaining the failure.</param>
    /// <param name="cancellationToken">Cancellation token for task cancellation.</param>
    Task ProcessFailedReservationExpirationAsync(string data,string errorMessage, CancellationToken cancellationToken);
}