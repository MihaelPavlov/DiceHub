using DH.Domain.Entities;

namespace DH.Domain.Services.Queue;

public interface IQueuedJobService : IDomainService<QueuedJob>
{
    Task Create(string queueName, string jobId, string payload, string? jobType = null);
    List<QueuedJob> GetJobsInPendingStatus();
    Task UpdatePayload(string queueName, string jobId, string payload);
    Task UpdateStatusToCompleted(string queueName, string jobId);
    Task UpdateStatusToFailed(string queueName, string jobId);
    Task UpdateStatusToCancelled(string queueName, string jobId);
    Task<List<QueuedJob>> GetJobsInPendingStatusByQueueType(string queueType, CancellationToken cancellationToken);
    Task<QueuedJob?> GetJobByJobId(string queueType, string jobId, CancellationToken cancellationToken);
}
