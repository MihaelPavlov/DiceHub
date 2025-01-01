namespace DH.Statistics.Domain.Entities;

public class RewardHistoryLog
{
    public int Id { get; set; }
    public int RewardId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public bool IsExpired { get; set; }
    public bool IsCollected { get; set; }
    public DateTime? CollectedDate { get; set; }
    public DateTime? ExpiredDate { get; set; }
    public DateTime CreatedDate { get; set; }
}
