namespace DH.Domain.Entities;

public class UserNotification
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string MessageId { get; set; } = string.Empty;
    public string PayloadJson { get; set; } = string.Empty;
    public string MessageType { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public bool HasBeenViewed { get; set; }
}