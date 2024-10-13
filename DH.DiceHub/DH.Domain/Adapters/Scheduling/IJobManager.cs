namespace DH.Domain.Adapters.Scheduling;

/// <summary>
/// Manages the creation and scheduling of jobs for processing specific tasks, such as expiring reservations.
/// </summary>
public interface IJobManager
{
    /// <summary>
    /// Creates and schedules a job that will expire a reservation after a specific duration.
    /// </summary>
    /// <param name="reservationId">The unique identifier of the reservation.</param>
    /// <param name="reservationTime">The time the reservation was made.</param>
    /// <param name="durationInMinutes">The duration in minutes after which the reservation will expire.</param>
    /// <returns>A task representing the asynchronous operation of scheduling the reservation expiration job.</returns>
    Task CreateReservationJob(int reservationId, DateTime reservationTime, int durationInMinutes);
}
