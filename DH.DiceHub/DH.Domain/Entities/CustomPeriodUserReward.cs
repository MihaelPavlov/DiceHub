namespace DH.Domain.Entities;

public class CustomPeriodUserReward
{
    public int Id { get; set; }
    public int RequiredPoints { get; set; }
    public bool IsCompleted { get; set; }

    public int RewardId { get; set; }
    public virtual ChallengeReward Reward { get; set; } = null!;

    public int UserChallengePeriodPerformanceId { get; set; }
    public virtual UserChallengePeriodPerformance UserChallengePeriodPerformance { get; set; } = null!;
}
