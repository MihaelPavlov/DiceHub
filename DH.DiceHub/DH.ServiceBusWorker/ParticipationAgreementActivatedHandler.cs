namespace DH.ServiceBusWorker;
public class ParticipationAgreementActivatedHandler : IServiceBusHandler<ParticipationAgreementActivatedMessage>
{
    public Task HandleMessageAsync(EventMessage<ParticipationAgreementActivatedMessage> message, string messageId, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Processing message: {message.Body.ParticipatioAgreementNumber}");
        return Task.CompletedTask;
    }
}