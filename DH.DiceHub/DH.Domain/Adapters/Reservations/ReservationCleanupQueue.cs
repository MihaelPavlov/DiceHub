using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace DH.Domain.Adapters.Reservations;

public class ReservationCleanupQueue
{
    // Concurrent queue to store job information.
    readonly ConcurrentQueue<JobInfo> queue = new();

    public void AddReservationCleaningJob(string reservationId, ReservationType type, DateTime removingTime)
    {
        queue.Enqueue(new JobInfo(reservationId, type, removingTime));
    }

    /// <summary>
    /// Attempts to dequeue a job from the queue.
    /// </summary>
    /// <param name="result">When this method returns, contains the job information if the operation was successful; otherwise, null.</param>
    /// <returns>True if a job was successfully dequeued; otherwise, false.</returns>
    public virtual bool TryDequeue([MaybeNullWhen(false)] out JobInfo result)
    {
        return queue.TryDequeue(out result);
    }

    public record JobInfo(string ReservationId, ReservationType Type, DateTime RemovingTime);
}
