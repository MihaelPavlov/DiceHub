using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Adapters.PushNotifications.Messages.Common;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;

namespace DH.Adapter.PushNotifications;

internal class PushNotificationsService : IPushNotificationsService
{
    readonly ILogger<PushNotificationsService> logger;

    public PushNotificationsService(ILogger<PushNotificationsService> logger)
    {
        this.logger = logger;
    }

    public async Task SendMessageAsync(MessageRequest message)
    {
        var notificationMessage = new Message
        {
            Token = message.DeviceToken,
            Notification = new Notification
            {
                Title = message.Title,
                Body = message.Body
            }
        };

        try
        {
            var responseId = await FirebaseMessaging.DefaultInstance.SendAsync(notificationMessage);

            if (string.IsNullOrEmpty(responseId))
                this.logger.LogWarning("Message from type {typeMessage}, was not send", typeof(MessageRequest));
        }
        catch (Exception ex)
        {

            throw;
        }

    }

    public async Task SendMultipleMessagesAsync(MultipleMessageRequest message)
    {
        var notificationMessage = new MulticastMessage
        {
            Tokens = message.Tokens,
            Notification = new Notification
            {
                Title = message.Title,
                Body = message.Body
            }
        };

        var response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(notificationMessage);

        this.logger.LogWarning("Sent message from type {TypeOfMessage}: {SuccessCount} successful, {FailureCount} failed.", typeof(MessageRequest), response.SuccessCount, response.FailureCount);
    }
}
