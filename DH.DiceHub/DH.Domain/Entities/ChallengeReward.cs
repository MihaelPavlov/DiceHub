using DH.Domain.Enums;

namespace DH.Domain.Entities;

public class ChallengeReward
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int RequiredPoints { get; set; }
    public RewardLevel Level { get; set; }

    public virtual ChallengeRewardImage Image { get; set; } = null!;
    public virtual ICollection<UserChallengeReward> UserRewards { get; set; } = [];
}
