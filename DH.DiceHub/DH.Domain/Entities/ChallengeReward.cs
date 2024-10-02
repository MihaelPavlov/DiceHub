using DH.Domain.Enums;

namespace DH.Domain.Entities;

public class ChallengeReward
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int RequiredPoints { get; set; }
    public RewardLevel Level { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string UpdatedBy { get; set; } = string.Empty;
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

    public virtual ChallengeRewardImage Image { get; set; } = null!;
    public virtual ICollection<UserChallengeReward> UserRewards { get; set; } = [];
}
