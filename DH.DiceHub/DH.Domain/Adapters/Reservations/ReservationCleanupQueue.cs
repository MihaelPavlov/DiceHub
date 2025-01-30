using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace DH.Domain.Adapters.Reservations;

public class ReservationCleanupQueue
{
    // Concurrent queue to store job information.
    readonly ConcurrentDictionary<int, JobInfo> jobs = new();

    public void AddReservationCleaningJob(int reservationId, ReservationType type, DateTime removingTime)
    {
        var jobInfo = new JobInfo(reservationId, type, removingTime);
        jobs.AddOrUpdate(reservationId, jobInfo, (id, existingJob) => jobInfo);
    }

    public void UpdateReservationCleaningJob(int reservationId, DateTime newRemovingTime)
    {
        if (jobs.TryGetValue(reservationId, out var existingJob))
        {
            var updatedJobInfo = existingJob with { RemovingTime = newRemovingTime };
            jobs.TryUpdate(reservationId, updatedJobInfo, existingJob);
        }
    }

    /// <summary>
    /// Attempts to dequeue a job from the queue.
    /// </summary>
    /// <param name="result">When this method returns, contains the job information if the operation was successful; otherwise, null.</param>
    /// <returns>True if a job was successfully dequeued; otherwise, false.</returns>
    public virtual bool TryDequeue([MaybeNullWhen(false)] out JobInfo result)
    {
        var nextJob = jobs.Values.OrderBy(j => j.RemovingTime).FirstOrDefault();
        if (nextJob != null && jobs.TryRemove(nextJob.ReservationId, out result))
        {
            return true;
        }
        result = null;
        return false;

    }

    public bool RemoveReservationCleaningJob(int reservationId)
    {
        return jobs.TryRemove(reservationId, out _);
    }

    public record JobInfo(int ReservationId, ReservationType Type, DateTime RemovingTime);
}
