namespace DH.Messaging.Publisher;

/// <summary>
/// Defines the contract for handling service bus messages of a specific type.
/// </summary>
/// <typeparam name="TMessage">The type of message being handled.</typeparam>
public interface IServiceBusHandler<TMessage>
{
    /// <summary>
    /// Handles a service bus message asynchronously.
    /// </summary>
    /// <param name="message">The message object containing the body and metadata.</param>
    /// <param name="messageId">The unique identifier of the message.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task HandleMessageAsync(EventMessage<TMessage> message, string messageId, CancellationToken cancellationToken);
}