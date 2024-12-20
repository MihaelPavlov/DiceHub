using DH.Messaging.Publisher;

namespace DH.Statistics.WorkerService;

public class PartProcessHandler : IServiceBusHandler<PartProcessMessage>
{
    public Task HandleMessageAsync(EventMessage<PartProcessMessage> message, string messageId, CancellationToken cancellationToken)
    {
        Console.WriteLine("hEllo");

        return Task.CompletedTask;
    }
}
