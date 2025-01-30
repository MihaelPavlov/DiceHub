﻿namespace DH.Domain.Queue;

/// <summary>
/// Represents the base class for all job information types.
/// </summary>
/// <remarks>
/// This abstract record serves as a foundation for all job-related data structures.  
/// Any queue implementation **must** define a corresponding job information type  
/// that inherits from <see cref="JobInfoBase"/>.  
/// </remarks>
/// <example>
/// To create a specific job type, inherit from <see cref="JobInfoBase"/>:
/// <code>
/// public record UserSyncJobInfo(string UserId, DateTime? ScheduledTime) : JobInfoBase;
/// </code>
/// </example>
public abstract record JobInfoBase
{
    /// <summary>
    /// Gets the unique identifier for the job.
    /// </summary>
    public Guid JobId { get; init; } = Guid.NewGuid();
}
