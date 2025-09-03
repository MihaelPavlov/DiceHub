using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Queue;
using DH.Domain.Services.Queue;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace DH.Domain.Adapters.Reservations;

public class ReservationCleanupQueue : QueueBase
{
    // FIFO queue
    private readonly ConcurrentQueue<JobInfo> queue = new();

    // Optional dictionary for fast lookups or updates
    private readonly ConcurrentDictionary<int, JobInfo> jobs = new();

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

        jobs[reservationId] = job; // Store for lookup/update
        queue.Enqueue(job);        // FIFO behavior
    }

    public void RequeueJob(JobInfo jobInfo)
    {
        if (jobs.TryGetValue(jobInfo.ReservationId, out var latestJob))
        {
            queue.Enqueue(latestJob);
        }
        else
        {
            queue.Enqueue(jobInfo);
            jobs[jobInfo.ReservationId] = jobInfo;
        }
    }

    public void UpdateReservationCleaningJob(int reservationId, DateTime newRemovingTime)
    {
        if (jobs.TryGetValue(reservationId, out var existingJob))
        {
            var updatedJobInfo = existingJob with { RemovingTime = newRemovingTime };
            jobs[reservationId] = existingJob with { RemovingTime = newRemovingTime };

            this.QueuedJobService.UpdatePayload(this.QueueName, updatedJobInfo.JobId, JsonSerializer.Serialize(updatedJobInfo));
        }
    }

    /// <summary>
    /// Attempts to dequeue a job from the queue.
    /// </summary>
    /// <param name="result">When this method returns, contains the job information if the operation was successful; otherwise, null.</param>
    /// <returns>True if a job was successfully dequeued; otherwise, false.</returns>
    public bool TryDequeue([MaybeNullWhen(false)] out JobInfo result)
    {
        if (queue.TryDequeue(out var jobInfo))
        {
            // Always refresh from dictionary if there’s an updated version
            if (jobs.TryGetValue(jobInfo.ReservationId, out var updatedJob))
            {
                result = updatedJob;
            }
            else
            {
                result = jobInfo;
            }
            return true;
        }

        result = null!;
        return false;
    }

    public bool RemoveReservationCleaningJob(int reservationId)
    {
        return jobs.TryRemove(reservationId, out _);
    }

    public record JobInfo(int ReservationId, ReservationType Type, DateTime RemovingTime) : JobInfoBase;
}
