using DH.Domain.Models.Common;
using DH.Domain.Services.Publisher;
using DH.Messaging.Publisher;

namespace DH.Application.Services;

public class EventPublisherService(RabbitMqOptions options, IRabbitMqClient rabbitMqClient) : IEventPublisherService
{
    readonly RabbitMqOptions options = options;
    readonly IRabbitMqClient rabbitMqClient = rabbitMqClient;

    public async Task PublishClubActivityDetectedMessage()
    {
        var message = new EventMessage<ClubActivityDetectedMessage>
        {
            MessageId = Guid.NewGuid().ToString(),
            DateTime = DateTimeOffset.UtcNow,
            Body = new ClubActivityDetectedMessage()
            {
                LogDate = DateTime.UtcNow
            }
        };

        await this.rabbitMqClient.Publish(options.ExchangeName, options.RoutingKeys.ClubActivityDetected, message);
    }
}
