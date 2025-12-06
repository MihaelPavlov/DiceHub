using DH.Domain.Entities;
using DH.Domain.Queue;
using DH.Domain.Services.Queue;
using System.Text.Json;

namespace DH.Domain.Adapters.ChallengesOrchestrator;

/// <summary>
/// Represents a queue for managing synchronization and challenge initiation jobs for users.
/// </summary>
public class SynchronizeUsersChallengesQueue(IQueuedJobService queuedJobService) : ISynchronizeUsersChallengesQueue
{
    public string QueueName => QueueNameKeysConstants.SYNCHRONIZE_USERS_CHALLENGES_QUEUE_NAME;

    readonly IQueuedJobService queuedJobService = queuedJobService;

    /// <summary>
    /// Adds a new synchronization job for a user to the queue.
    /// </summary>
    /// <param name="userId">The unique identifier for the user whose synchronization job is being added.</param>
    /// <param name="scheduledTime">The time when the synchronization job is scheduled to execute.</param>
    public async Task AddSynchronizeNewUserJob(string userId)
    {
        var job = new SynchronizeNewUserJob(userId);
        await this.queuedJobService.Create(this.QueueName, job.JobId, JsonSerializer.Serialize(job));
    }

    /// <summary>
    /// Adds a new challenge initiation job for a user to the queue.
    /// </summary>
    /// <param name="userId">The unique identifier for the user.</param>
    ///     /// <param name="scheduledTime">The time when the synchronization job is scheduled to execute.</param>
    public async Task AddChallengeInitiationJob(string userId, DateTime scheduledTime)
    {
        var job = new ChallengeInitiationJob(userId, scheduledTime);
        await this.queuedJobService.Create(this.QueueName, job.JobId, JsonSerializer.Serialize(job));
    }

    /// <summary>
    /// Attempts to dequeue a job from the queue.
    /// </summary>
    /// <param name="result">When this method returns, contains the job information if the operation was successful; otherwise, null.</param>
    /// <returns>True if a job was successfully dequeued; otherwise, false.</returns>
    public async Task<List<QueuedJob>> TryDequeue(CancellationToken cancellationToken)
    {
        var queuedJobs = await this.queuedJobService.GetJobsInPendingStatusByQueueType(this.QueueName, cancellationToken);

        return queuedJobs ?? [];
    }
}
