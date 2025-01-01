namespace DH.Messaging.Publisher.Messages;

public class RewardActionDetectedMessage
{
    public string UserId { get; set; } = string.Empty;
    public int RewardId { get; set; }
    public bool IsExpired { get; set; }
    public bool IsCollected { get; set; }
    public DateTime ActionDate { get; set; }
}
