using DH.Domain.Models.Common;
using DH.Domain.Services.Publisher;
using DH.Messaging.Publisher;
using DH.Messaging.Publisher.Messages;

namespace DH.Application.Services;

public class EventPublisherService(RabbitMqOptions options, IRabbitMqClient rabbitMqClient) : IEventPublisherService
{
    readonly RabbitMqOptions options = options;
    readonly IRabbitMqClient rabbitMqClient = rabbitMqClient;

    public async Task PublishClubActivityDetectedMessage(string? userId = null)
    {
        var message = new EventMessage<ClubActivityDetectedMessage>
        {
            MessageId = Guid.NewGuid().ToString(),
            DateTime = DateTimeOffset.UtcNow,
            Body = new ClubActivityDetectedMessage()
            {
                UserId = userId,
                LogDate = DateTime.UtcNow
            }
        };

        await this.rabbitMqClient.Publish(options.ExchangeName, options.RoutingKeys.ClubActivityDetected, message);
    }

    public async Task PublishEventAttendanceDetectedMessage(string action, int eventId)
    {
        if (Enum.TryParse<AttendanceAction>(action, out var parsedAction))
        {
            var message = new EventMessage<EventAttendanceDetectedMessage>
            {
                MessageId = Guid.NewGuid().ToString(),
                DateTime = DateTimeOffset.UtcNow,
                Body = new EventAttendanceDetectedMessage()
                {
                    LogDate = DateTime.UtcNow,
                    EventId = eventId,
                    Type = parsedAction
                }
            };

            await this.rabbitMqClient.Publish(options.ExchangeName, options.RoutingKeys.EventAttendanceDetected, message);
        }
        else //TODO: Added a Logger
            return;

    }

    public async Task PublishReservationProcessingOutcomeMessage(string action, string userId, string type, int reservationId)
    {
        if (Enum.TryParse<ReservationOutcome>(action, out var parsedAction) && Enum.TryParse<ReservationType>(type, out var parsedType))
        {
            var message = new EventMessage<ReservationProcessingOutcomeMessage>
            {
                MessageId = Guid.NewGuid().ToString(),
                DateTime = DateTimeOffset.UtcNow,
                Body = new ReservationProcessingOutcomeMessage()
                {
                    UserId = userId,
                    OutcomeDate = DateTime.UtcNow,
                    ReservationId = reservationId,
                    Outcome = parsedAction,
                    Type = parsedType
                }
            };

            await this.rabbitMqClient.Publish(options.ExchangeName, options.RoutingKeys.ReservationProcessingOutcome, message);
        }
        else //TODO: Added a Logger
            return;
    }

    public async Task PublishRewardActionDetectedMessage(string userId, int rewardId, bool isExpired, bool isCollected)
    {
        var message = new EventMessage<RewardActionDetectedMessage>
        {
            MessageId = Guid.NewGuid().ToString(),
            DateTime = DateTimeOffset.UtcNow,
            Body = new RewardActionDetectedMessage()
            {
                UserId = userId,
                ActionDate = DateTime.UtcNow,
                IsCollected = isCollected,
                IsExpired = isExpired,
                RewardId = rewardId
            }
        };

        await this.rabbitMqClient.Publish(options.ExchangeName, options.RoutingKeys.RewardActionDetected, message);
    }

    public async Task PublishChallengeProcessingOutcomeMessage(string userId, int challengeId, string type)
    {
        if (Enum.TryParse<ChallengeOutcome>(type, out var parsedType))
        {
            var message = new EventMessage<ChallengeProcessingOutcomeMessage>
            {
                MessageId = Guid.NewGuid().ToString(),
                DateTime = DateTimeOffset.UtcNow,
                Body = new ChallengeProcessingOutcomeMessage()
                {
                    UserId = userId,
                    OutcomeDate = DateTime.UtcNow,
                    ChallengeId = challengeId,
                    Outcome = parsedType,
                }
            };
            await this.rabbitMqClient.Publish(options.ExchangeName, options.RoutingKeys.ChallengeProcessingOutcome, message);
        }
        else //TODO: Added a Logger
            return;
    }
}
