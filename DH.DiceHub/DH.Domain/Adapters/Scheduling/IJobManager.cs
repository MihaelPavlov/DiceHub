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

    /// <summary>
    /// Deletes a job from the scheduler based on the job name and optionally its group.
    /// If no group is specified, the default group ("DEFAULT") will be used.
    /// </summary>
    /// <param name="jobName">The name of the job to be deleted.</param>
    /// <param name="jobGroup">The group to which the job belongs (optional, defaults to "DEFAULT").</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<bool> DeleteJob(string jobName, string jobGroup = "DEFAULT");
}
