namespace DH.Domain.Adapters.PushNotifications.Messages.Models;

public class GetUserNotificationsModel
{
    public int Id { get; set; }
    public string MessageId { get; set; } = string.Empty;
    public string MessageBody { get; set; } = string.Empty;
    public string MessageTitle { get; set; } = string.Empty;
    public string MessageType { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public bool HasBeenViewed { get; set; }
}
