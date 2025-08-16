namespace DH.Domain.Adapters.PushNotifications.Messages.Models;

public class NotificationPayload
{
    public required string Body { get; set; }
    public required string Title { get; set; }
}