using DH.Messaging.Publisher;

namespace DH.Statistics.WorkerService;

public class ParticipationAgreementActivatedHandler : IServiceBusHandler<ParticipationAgreementActivatedMessage>
{
    public Task HandleMessageAsync(EventMessage<ParticipationAgreementActivatedMessage> message, string messageId, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Processing message: ");
        return Task.CompletedTask;
    }
}