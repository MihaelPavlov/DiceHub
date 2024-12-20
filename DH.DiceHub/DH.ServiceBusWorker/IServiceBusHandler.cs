namespace DH.ServiceBusWorker;

public interface IServiceBusHandler<TMessage>
{
    Task HandleMessageAsync(EventMessage<TMessage> message, string messageId, CancellationToken cancellationToken);
}