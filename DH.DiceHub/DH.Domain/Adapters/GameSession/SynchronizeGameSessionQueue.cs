using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DH.Domain.Adapters.GameSession;

/// <summary>
/// Represents a queue that manages jobs for enforcing user playtime requirements during game sessions.
/// This queue ensures that users spend the necessary average time playing a game before they can complete related challenges.
/// </summary>
public class SynchronizeGameSessionQueue
{
    // Concurrent queue to store job information.
    readonly ConcurrentQueue<JobInfo> queue = new();

    // HashSet to store job identifiers for canceled jobs
    private readonly HashSet<(string UserId, int GameId)> canceledJobs = new();

    public IReadOnlyCollection<(string UserId, int GameId)> CanceledJobs => canceledJobs;

    /// <summary>
    /// Adds a new user playtime enforcement job to the queue.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose playtime needs to be enforced.</param>
    /// <param name="gameId">The unique identifier of the game for which the playtime enforcement applies.</param>
    /// <param name="averageTimePlay">The average playtime that the user must meet to complete the challenge.</param>
    public void AddUserPlayTimEnforcerJob(string userId, int gameId, DateTime requiredPlayUntil)
    {
        queue.Enqueue(new UserPlayTimeEnforcerJob(userId, gameId, requiredPlayUntil));
    }

    /// <summary>
    /// Check if queue contains job with key userId and gameId
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose playtime needs to be enforced.</param>
    /// <param name="gameId">The unique identifier of the game for which the playtime enforcement applies.</param>
    public bool Contains(string userId, int gameId)
    {
       return queue.Any(x => x.UserId == userId && x.GameId == gameId);
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

    /// <summary>
    /// Cancels a user playtime enforcement job.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose job needs to be canceled.</param>
    /// <param name="gameId">The unique identifier of the game associated with the job.</param>
    public void CancelUserPlayTimeEnforcerJob(string userId, int gameId)
    {
        this.canceledJobs.Add((userId, gameId));
    }

    /// <summary>
    /// Remove the cancel job record after the job is not processed.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose job needs to be canceled.</param>
    /// <param name="gameId">The unique identifier of the game associated with the job.</param>
    public void RemoveRecordFromCanceledJob(string userId, int gameId)
    {
        this.canceledJobs.Remove((userId, gameId));
    }

    /// <summary>
    /// Represents the basic information related to a job for enforcing user playtime.
    /// </summary>
    /// <param name="UserId">The unique identifier of the user associated with the job.</param>
    /// <param name="GameId">The unique identifier of the game that the job is related to.</param>
    /// <param name="requiredPlayUntil">The average time that the user must spend playing the game.</param>
    public record JobInfo(string UserId, int GameId, DateTime requiredPlayUntil);

    /// <summary>
    /// Represents a job specifically for enforcing user playtime requirements for a game.
    /// </summary>
    /// <param name="UserId">The unique identifier of the user.</param>
    /// <param name="GameId">The unique identifier of the game.</param>
    /// <param name="AverageTimePlay">The average time that the user is required to play the game.</param>
    public record UserPlayTimeEnforcerJob(string UserId, int GameId, DateTime RequiredPlayUntil) : JobInfo(UserId, GameId, RequiredPlayUntil);
}
