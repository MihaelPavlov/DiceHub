using DH.Domain.Queue;

namespace DH.Domain.Adapters.ChallengesOrchestrator;

/// <summary>
/// Represents a template for job information related to user synchronization.
/// </summary>
/// <param name="UserId">The unique identifier for the user associated with the job.</param>
/// <param name="ScheduledTime">The scheduled time for the job's execution. 
/// If set to NULL, the job will be executed immediately.</param>
public record JobInfo(string TypeOfJob, string UserId, DateTime? ScheduledTime) 
    : JobInfoBase(SynchronizeUsersChallengesQueueHelper.BuildJobId(TypeOfJob, UserId));

/// <summary>
/// Represents a job for initiating a challenge for a user.
/// </summary>
/// <param name="UserId">The unique identifier for the user.</param>
///     /// <param name="ScheduledTime">The time when the synchronization job will be executed. 
/// If not specified, defaults to 6 hours from the current time.</param>
//TODO: Those 6 hours should come from options somewhere maybe appsettings
public record ChallengeInitiationJob(string UserId, DateTime? ScheduledTime) 
    : JobInfo(nameof(ChallengeInitiationJob), UserId, ScheduledTime ?? DateTime.UtcNow.AddHours(6));

/// <summary>
/// Represents a job for synchronizing a new user.
/// </summary>
/// <param name="UserId">The unique identifier for the new user.</param>
public record SynchronizeNewUserJob(string UserId) 
    : JobInfo(nameof(SynchronizeNewUserJob), UserId, null);
