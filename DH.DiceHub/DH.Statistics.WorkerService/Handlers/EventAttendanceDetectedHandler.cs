using DH.Messaging.HttpClient.Enums;
using DH.Messaging.HttpClient;
using DH.Messaging.Publisher;
using DH.OperationResultCore.Utility;
using DH.Messaging.Publisher.Messages;

namespace DH.Statistics.WorkerService.Handlers;

internal class EventAttendanceDetectedHandler(IAuthorizedClientFactory authorizedClientFactory, IRabbitMqClient client) : IServiceBusHandler<EventAttendanceDetectedMessage>
{
    readonly IAuthorizedClientFactory _authorizedClientFactory = authorizedClientFactory;
    readonly IRabbitMqClient _client = client;

    public async Task HandleMessageAsync(EventMessage<EventAttendanceDetectedMessage> message, string messageId, CancellationToken cancellationToken)
    {
        if (message.Body is null)
            throw new Exception("Body is Empty or Null");

        var sender = _client.GetSender();
        OperationResult<int>? result = null;

        if (message.Body.Type == AttendanceAction.Joining)
        {
            result = await _authorizedClientFactory
              .CreateClient(ApplicationApi.Statistics, sender.UserId, sender.Token)
              .BuildPost(ApiEndpoints.Statistics.CreateEventAttendanceLog)
              .WithContent(new { sender.UserId, message.Body.LogDate, message.Body.EventId })
              .WithImpersonated()
              .SendWithResulAsync<OperationResult<int>>(cancellationToken);
        }
        else if (message.Body.Type == AttendanceAction.Leaving)
        {
            result = await _authorizedClientFactory
             .CreateClient(ApplicationApi.Statistics, sender.UserId, sender.Token)
             .BuildDelete(ApiEndpoints.Statistics.RemoveEventAttendanceLog)
             .WithContent(new { sender.UserId, message.Body.EventId })
             .WithImpersonated()
             .SendWithResulAsync<OperationResult<int>>(cancellationToken);
        }

        if (result is null || !result.Success)
            throw new Exception("The handler was not handle successfully");
    }
}
