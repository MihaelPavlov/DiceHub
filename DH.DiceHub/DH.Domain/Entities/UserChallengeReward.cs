namespace DH.Domain.Entities;

public class UserChallengeReward
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int RewardId { get; set; }
    public bool IsClaimed { get; set; }
    public DateTime ClaimedDate { get; set; }
    public DateTime AvailableFromDate { get; set; }
    public DateTime ExpiresDate { get; set; }

    public virtual ChallengeReward Reward { get; set; } = null!;
}
