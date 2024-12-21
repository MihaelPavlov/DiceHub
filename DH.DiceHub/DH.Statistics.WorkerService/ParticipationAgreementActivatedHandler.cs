using DH.Messaging.Publisher;

namespace DH.Statistics.WorkerService;

public class ParticipationAgreementActivatedHandler(IRabbitMqClient client) : IServiceBusHandler<ParticipationAgreementActivatedMessage>
{
    readonly IRabbitMqClient client = client;

    public async Task HandleMessageAsync(EventMessage<ParticipationAgreementActivatedMessage> message, string messageId, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Processing message: ");

        await client.Publish("exchange", "routingKey", new ParticipationAgreementActivatedMessage());
    }
}