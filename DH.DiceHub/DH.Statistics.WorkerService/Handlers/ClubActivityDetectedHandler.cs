using DH.Messaging.HttpClient;
using DH.Messaging.HttpClient.Enums;
using DH.Messaging.Publisher;
using DH.OperationResultCore.Utility;

namespace DH.Statistics.WorkerService.Handlers;

public class ClubActivityDetectedHandler(IAuthorizedClientFactory authorizedClientFactory, IRabbitMqClient client) : IServiceBusHandler<ClubActivityDetectedMessage>
{
    readonly IAuthorizedClientFactory _authorizedClientFactory = authorizedClientFactory;
    readonly IRabbitMqClient _client = client;

    public async Task HandleMessageAsync(EventMessage<ClubActivityDetectedMessage> message, string messageId, CancellationToken cancellationToken)
    {
        if (message.Body is null)
            throw new Exception("Body is Empty or Null");

        var sender = _client.GetSender();
        var result = await _authorizedClientFactory
           .CreateClient(ApplicationApi.Statistics, sender.UserId, sender.Token)
           .BuildPost(ApiEndpoints.Statistics.CreateClubActivityLog)
           .WithContent(new { sender.UserId, message.Body.LogDate })
           .WithImpersonated()
           .SendWithResulAsync<OperationResult<int>>(cancellationToken);

        if (!result.Success)
            throw new Exception("The handler was not handle successfully");
    }
}
