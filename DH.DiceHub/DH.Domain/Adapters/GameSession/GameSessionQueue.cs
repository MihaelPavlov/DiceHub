using DH.Domain.Entities;
using DH.Domain.Queue;
using DH.Domain.Services.Queue;
using System.Text.Json;

namespace DH.Domain.Adapters.GameSession;

/// <summary>
/// Represents a queue that manages jobs for enforcing user playtime requirements during game sessions.
/// This queue ensures that users spend the necessary average time playing a game before they can complete related challenges.
/// </summary>
public class GameSessionQueue(IQueuedJobService queuedJobService) : IGameSessionQueue
{
    public string QueueName => QueueNameKeysConstants.SYNCHRONIZE_GAME_SESSION_QUEUE_NAME;

    readonly IQueuedJobService queuedJobService = queuedJobService;

    /// <summary>
    /// Adds a new user playtime enforcement job to the queue.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose playtime needs to be enforced.</param>
    /// <param name="gameId">The unique identifier of the game for which the playtime enforcement applies.</param>
    /// <param name="averageTimePlay">The average playtime that the user must meet to complete the challenge.</param>
    public async Task AddUserPlayTimEnforcerJob(string userId, int gameId, DateTime requiredPlayUntil)
    {
        var job = new UserPlayTimeEnforcerJob(userId, gameId, requiredPlayUntil);
        await this.queuedJobService.Create(this.QueueName, job.JobId, JsonSerializer.Serialize(job));
    }

    /// <summary>
    /// Check if queue contains job with key userId and gameId
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose playtime needs to be enforced.</param>
    /// <param name="gameId">The unique identifier of the game for which the playtime enforcement applies.</param>
    public async Task<bool> Contains(string userId, int gameId, CancellationToken cancellationToken)
    {
        var jobId = GameSessionHelper.BuildJobId(userId, gameId);
        var job = await this.queuedJobService.GetJobByJobId(this.QueueName, jobId, cancellationToken);

        return job != null ? true : false;
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

    /// <summary>
    /// Cancels a user playtime enforcement job.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose job needs to be canceled.</param>
    /// <param name="gameId">The unique identifier of the game associated with the job.</param>
    public async Task CancelUserPlayTimeEnforcerJob(string userId, int gameId)
    {
        var jobId = GameSessionHelper.BuildJobId(userId, gameId);
        await this.queuedJobService.UpdateStatusToCancelled(this.QueueName, jobId);
    }
}
