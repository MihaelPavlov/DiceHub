using DH.Domain.Queue;
using DH.Domain.Services.Queue;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace DH.Domain.Adapters.Reservations;

public class ReservationCleanupQueue : QueueBase
{
    // Concurrent queue to store job information.
    readonly ConcurrentDictionary<int, JobInfo> jobs = new();

    public override string QueueName => QueueNameKeysConstants.RESERVATION_CLEANUP_QUEUE_NAME;

    private IQueuedJobService? queuedJobService = null;
    readonly IServiceScopeFactory serviceFactory;
    public ReservationCleanupQueue(IServiceScopeFactory serviceFactory)
    {
        this.serviceFactory = serviceFactory;
    }

    private IQueuedJobService QueuedJobService
    {
        get
        {
            if (queuedJobService == null)
            {
                queuedJobService = serviceFactory.CreateScope().ServiceProvider.GetRequiredService<IQueuedJobService>();
            }
            return queuedJobService;
        }
    }

    public void AddReservationCleaningJob(int reservationId, ReservationType type, DateTime removingTime)
    {
        var job = new JobInfo(reservationId, type, removingTime);
        this.QueuedJobService.Create(this.QueueName, job.JobId, JsonSerializer.Serialize(job));
        jobs.AddOrUpdate(reservationId, job, (id, existingJob) => job);
    }

    public void RequeueJob(JobInfo jobInfo)
    {
        jobs.AddOrUpdate(jobInfo.ReservationId, jobInfo, (id, existingJob) => jobInfo);
    }

    public void UpdateReservationCleaningJob(int reservationId, DateTime newRemovingTime)
    {
        if (jobs.TryGetValue(reservationId, out var existingJob))
        {
            var updatedJobInfo = existingJob with { RemovingTime = newRemovingTime };
            var isUpdated = jobs.TryUpdate(reservationId, updatedJobInfo, existingJob);

            if (isUpdated)
                this.QueuedJobService.UpdatePayload(this.QueueName, updatedJobInfo.JobId, JsonSerializer.Serialize(updatedJobInfo));
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

    public record JobInfo(int ReservationId, ReservationType Type, DateTime RemovingTime) : JobInfoBase;
}
