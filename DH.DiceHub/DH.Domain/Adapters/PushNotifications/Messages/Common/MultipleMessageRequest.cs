namespace DH.Domain.Adapters.PushNotifications.Messages.Common;

public class MultipleMessageRequest
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public List<string> Tokens { get; set; } = [];
}
