namespace DH.Domain.Entities;

public class UserChallengePeriodReward : TenantEntity
{
    public int Id { get; set; }
    public int ChallengeRewardId { get; set; }
    public int UserChallengePeriodPerformanceId { get; set; }
    public bool IsCompleted { get; set; }

    public virtual ChallengeReward ChallengeReward { get; set; } = null!;
    public virtual UserChallengePeriodPerformance UserChallengePeriodPerformance { get; set; } = null!;
}
