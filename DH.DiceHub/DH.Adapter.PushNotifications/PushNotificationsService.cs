using DH.Domain.Adapters.PushNotifications;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;

namespace DH.Adapter.PushNotifications;

internal class PushNotificationsService : IPushNotificationsService
{
    public async Task SendMessage(MessageRequest message)
    {
        var notification = new Message()
        {
            Notification = new Notification
            {
                Title = message.Title,
                Body = message.Body,
            },
            Data = new Dictionary<string, string>()
            {
                ["FirstName"] = "John",
                ["LastName"] = "Doe"
            },
            Token = message.DeviceToken
        };
        
        var messaging = FirebaseMessaging.DefaultInstance;
        string result;
        try
        {
         result = await messaging.SendAsync(notification);

        }
        catch (Exception ex)
        {

            throw;
        }

        if (!string.IsNullOrEmpty(result))
        {
            // Message was sent successfully
            return;
        }
        else
        {
            // There was an error sending the message
            throw new Exception("Error sending the message.");
        }
    }
}
