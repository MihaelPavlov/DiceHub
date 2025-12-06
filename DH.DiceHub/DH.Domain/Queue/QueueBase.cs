namespace DH.Domain.Queue;

/// <summary>
/// Represents the base class for all queue implementations.
/// </summary>
/// <remarks>
/// This abstract class serves as the foundation for all queue types.  
/// Any specific queue implementation **must** inherit from <see cref="QueueBase"/>  
/// and specify the <see cref="QueueName"/> property to identify the queue.
/// </remarks>
/// <example>
/// To create a specific queue, inherit from <see cref="QueueBase"/> and override the <see cref="QueueName"/>:
/// <code>
/// public class UserSyncQueue : QueueBase
/// {
///     public override string QueueName => "user-sync-queue";
/// }
/// </code>
/// </example>
public interface QueueBase
{
    /// <summary>
    /// Gets the name of the queue.
    /// </summary>
    /// <remarks>
    /// This property must be overridden by any queue implementation to provide a unique name 
    /// for the queue, which is used for processing jobs and identifying the queue in the system.
    /// </remarks>
    public string QueueName { get; }
}
