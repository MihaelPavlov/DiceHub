namespace DH.Domain.Entities;

public class UserChallengeReward : TenantEntity
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int RewardId { get; set; }
    public bool IsClaimed { get; set; }
    public bool IsExpired { get; set; }

    // The date which track from when the reward is available for claiming.
    public DateTime AvailableFromDate { get; set; }

    // The date when the reward is claimed;
    public DateTime? ClaimedDate { get; set; }

    // The date till when the reward can be claimed
    public DateTime ExpiresDate { get; set; }

    public virtual ChallengeReward Reward { get; set; } = null!;
}
