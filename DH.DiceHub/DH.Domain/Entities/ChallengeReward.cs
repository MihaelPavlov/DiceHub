using DH.Domain.Enums;

namespace DH.Domain.Entities;

public class ChallengeReward
{
    public int Id { get; set; }
    public string Name_EN { get; set; } = string.Empty;
    public string Name_BG { get; set; } = string.Empty;
    public decimal CashEquivalent { get; set; }
    public string Description_EN { get; set; } = string.Empty;
    public string Description_BG { get; set; } = string.Empty;
    public RewardRequiredPoint RequiredPoints { get; set; }
    public RewardLevel Level { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string UpdatedBy { get; set; } = string.Empty;
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }
    public string ImageUrl { get; set; } = string.Empty;

    public virtual ICollection<UserChallengeReward> UserRewards { get; set; } = [];
    public virtual ICollection<UserChallengePeriodReward> UserChallengePeriodRewards { get; set; } = [];
}
