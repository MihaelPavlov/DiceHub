using DH.Messaging.HttpClient;
using DH.Messaging.Publisher.Messages;
using DH.Messaging.Publisher;
using DH.Messaging.HttpClient.Enums;
using DH.OperationResultCore.Utility;

namespace DH.Statistics.WorkerService.Handlers;

public record RewardActionDetectedHandler(IAuthorizedClientFactory authorizedClientFactory, IRabbitMqClient client) : IServiceBusHandler<RewardActionDetectedMessage>
{
    readonly IAuthorizedClientFactory _authorizedClientFactory = authorizedClientFactory;
    readonly IRabbitMqClient _client = client;

    public async Task HandleMessageAsync(EventMessage<RewardActionDetectedMessage> message, string messageId, CancellationToken cancellationToken)
    {
        if (message.Body is null)
            throw new Exception("Body is Empty or Null");

        var sender = _client.GetSender();

        var request = _authorizedClientFactory
            .CreateClient(ApplicationApi.Statistics, sender.UserId, sender.Token)
            .BuildPost(ApiEndpoints.Statistics.CreateRewardHistoryLog);

        if (message.Body.IsExpired)
        {
            request.WithContent(new { message.Body.UserId, message.Body.RewardId, message.Body.IsExpired, ExpiredDate = message.Body.ActionDate });
        }
        else if (message.Body.IsCollected)
        {
            request.WithContent(new { message.Body.UserId, message.Body.RewardId, message.Body.IsCollected, CollectedDate = message.Body.ActionDate });
        }

        var result = await request
             .WithImpersonated()
             .SendWithResulAsync<OperationResult<int>>(cancellationToken);

        //TODO: What happened after failed
    }
}
