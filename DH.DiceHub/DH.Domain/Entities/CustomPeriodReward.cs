namespace DH.Domain.Entities;

public class CustomPeriodReward : TenantEntity
{
    public int Id { get; set; }
    public int RequiredPoints { get; set; }

    public int RewardId { get; set; }
    public virtual ChallengeReward Reward { get; set; } = null!;
}
