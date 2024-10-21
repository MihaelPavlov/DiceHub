namespace DH.Domain.Adapters.PushNotifications;

public class MessageRequest
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string DeviceToken { get; set; } = string.Empty;
}
