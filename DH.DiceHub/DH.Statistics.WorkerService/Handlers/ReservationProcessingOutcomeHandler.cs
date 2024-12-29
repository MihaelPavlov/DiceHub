using DH.Messaging.HttpClient;
using DH.Messaging.HttpClient.Enums;
using DH.Messaging.Publisher;
using DH.Messaging.Publisher.Messages;
using DH.OperationResultCore.Utility;

namespace DH.Statistics.WorkerService.Handlers;

internal class ReservationProcessingOutcomeHandler(IAuthorizedClientFactory authorizedClientFactory, IRabbitMqClient client) : IServiceBusHandler<ReservationProcessingOutcomeMessage>
{
    readonly IAuthorizedClientFactory _authorizedClientFactory = authorizedClientFactory;
    readonly IRabbitMqClient _client = client;

    public async Task HandleMessageAsync(EventMessage<ReservationProcessingOutcomeMessage> message, string messageId, CancellationToken cancellationToken)
    {
        if (message.Body is null)
            throw new Exception("Body is Empty or Null");

        var sender = _client.GetSender();

        var result = await _authorizedClientFactory
             .CreateClient(ApplicationApi.Statistics, sender.UserId, sender.Token)
             .BuildPost(ApiEndpoints.Statistics.CreateEventAttendanceLog)
             .WithContent(new { message.Body.UserId, message.Body.ReservationId, message.Body.Outcome, message.Body.OutcomeDate, message.Body.Type })
             .WithImpersonated()
             .SendWithResulAsync<OperationResult<int>>(cancellationToken);
    }
}
