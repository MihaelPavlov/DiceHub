using DH.Messaging.HttpClient;
using DH.Messaging.Publisher.Messages;
using DH.Messaging.Publisher;
using DH.Messaging.HttpClient.Enums;
using DH.OperationResultCore.Utility;

namespace DH.Statistics.WorkerService.Handlers;

internal class ChallengeProcessingOutcomeHandler(IAuthorizedClientFactory authorizedClientFactory, IRabbitMqClient client) : IServiceBusHandler<ChallengeProcessingOutcomeMessage>
{
    readonly IAuthorizedClientFactory _authorizedClientFactory = authorizedClientFactory;
    readonly IRabbitMqClient _client = client;

    public async Task HandleMessageAsync(EventMessage<ChallengeProcessingOutcomeMessage> message, string messageId, CancellationToken cancellationToken)
    {
        if (message.Body is null)
            throw new Exception("Body is Empty or Null");

        var sender = _client.GetSender();

        var request = await _authorizedClientFactory
            .CreateClient(ApplicationApi.Statistics, sender.UserId, sender.Token)
            .BuildPost(ApiEndpoints.Statistics.CreateRewardHistoryLog)
            .WithContent(new { message.Body.UserId, message.Body.Outcome, message.Body.OutcomeDate, message.Body.ChallengeId })
            .WithImpersonated()
            .SendWithResulAsync<OperationResult<int>>(cancellationToken);

        //TODO: What happened after failed
    }
}
