using DH.Domain.Entities;

namespace DH.Domain.Services.Queue;

public interface IQueuedJobService : IDomainService<QueuedJob>
{
    Task Create(string queueName, Guid jobId, string payload);
    Task UpdatePayload(string queueName, Guid jobId, string payload);
    Task UpdateStatusToCompleted(string queueName, Guid jobId);
    Task UpdateStatusToFailed(string queueName, Guid jobId);
    Task UpdateStatusToCancelled(string queueName, Guid jobId);
}
