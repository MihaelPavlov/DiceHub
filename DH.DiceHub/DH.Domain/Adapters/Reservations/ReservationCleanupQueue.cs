using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Queue;
using DH.Domain.Services.Queue;
using System.Text.Json;

namespace DH.Domain.Adapters.Reservations;

public class ReservationCleanupQueue(IQueuedJobService queuedJobService) : IReservationCleanupQueue
{
    public string QueueName => QueueNameKeysConstants.RESERVATION_CLEANUP_QUEUE_NAME;

    readonly IQueuedJobService queuedJobService = queuedJobService;

    public async Task AddReservationCleaningJob(int reservationId, ReservationType type, DateTime removingTime)
    {
        var job = new ReservationCleanupJobInfo(reservationId, type, removingTime);
        await this.queuedJobService.Create(this.QueueName, job.JobId, JsonSerializer.Serialize(job));
    }

    public async Task UpdateReservationCleaningJob(int reservationId, ReservationType type, DateTime newRemovingTime)
    {
        var job = new ReservationCleanupJobInfo(reservationId, type, newRemovingTime);

        await this.queuedJobService.UpdatePayload(this.QueueName, job.JobId, JsonSerializer.Serialize(job));
    }

    public async Task<List<QueuedJob>> TryDequeue(CancellationToken cancellationToken)
    {
        var queuedJobs = await this.queuedJobService.GetJobsInPendingStatusByQueueType(this.QueueName, cancellationToken);

        return queuedJobs ?? [];
    }

    public async Task CancelReservationCleaningJob(int reservationId, ReservationType type)
    {
        var jobId = ReservationCleanupHelper.BuildJobId(reservationId, type);
        await this.queuedJobService.UpdateStatusToCancelled(this.QueueName, jobId);
    }
}
