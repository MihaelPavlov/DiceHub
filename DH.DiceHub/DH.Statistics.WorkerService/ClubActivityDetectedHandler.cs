using DH.Messaging.HttpClient;
using DH.Messaging.HttpClient.Enums;
using DH.Messaging.Publisher;

namespace DH.Statistics.WorkerService;

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
           .WithContent(new { UserId = sender.UserId, message.Body.LogDate })
           .WithImpersonated()
           .SendWithResulAsync<object>(cancellationToken);

        //TODO: Use the operation result after move it to library
    }
}
