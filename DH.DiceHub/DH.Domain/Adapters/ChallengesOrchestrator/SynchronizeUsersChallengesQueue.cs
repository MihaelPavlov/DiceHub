﻿using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace DH.Domain.Adapters.ChallengesOrchestrator;

/// <summary>
/// Represents a queue for managing synchronization and challenge initiation jobs for users.
/// </summary>
public class SynchronizeUsersChallengesQueue
{
    // Concurrent queue to store job information.
    readonly ConcurrentQueue<JobInfo> queue = new();

    /// <summary>
    /// Adds a new synchronization job for a user to the queue.
    /// </summary>
    /// <param name="userId">The unique identifier for the user whose synchronization job is being added.</param>
    /// <param name="scheduledTime">The time when the synchronization job is scheduled to execute.</param>
    public void AddSynchronizeNewUserJob(string userId)
    {
        queue.Enqueue(new SynchronizeNewUserJob(userId));
    }

    /// <summary>
    /// Adds a new challenge initiation job for a user to the queue.
    /// </summary>
    /// <param name="userId">The unique identifier for the user.</param>
    ///     /// <param name="scheduledTime">The time when the synchronization job is scheduled to execute.</param>
    public void AddChallengeInitiationJob(string userId, DateTime scheduledTime)
    {
        queue.Enqueue(new ChallengeInitiationJob(userId,scheduledTime));
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
    /// Represents a template for job information related to user synchronization.
    /// </summary>
    /// <param name="UserId">The unique identifier for the user associated with the job.</param>
    /// <param name="ScheduledTime">The scheduled time for the job's execution. 
    /// If set to NULL, the job will be executed immediately.</param>
    public record JobInfo(string UserId, DateTime? ScheduledTime);

    /// <summary>
    /// Represents a job for initiating a challenge for a user.
    /// </summary>
    /// <param name="UserId">The unique identifier for the user.</param>
    ///     /// <param name="ScheduledTime">The time when the synchronization job will be executed. 
    /// If not specified, defaults to 6 hours from the current time.</param>
    //TODO: Those 6 hours should come from options somewhere maybe appsettings
    public record ChallengeInitiationJob(string UserId, DateTime? ScheduledTime) : JobInfo(UserId, ScheduledTime ?? DateTime.UtcNow.AddHours(6));

    /// <summary>
    /// Represents a job for synchronizing a new user.
    /// </summary>
    /// <param name="UserId">The unique identifier for the new user.</param>
    public record SynchronizeNewUserJob(string UserId) : JobInfo(UserId, null);
}