namespace DH.Domain.Entities;

public class CustomPeriodUserChallenge : TenantEntity
{
    public int Id { get; set; }
    public int ChallengeAttempts { get; set; }
    public int UserAttempts { get; set; }
    public int RewardPoints { get; set; }
    public bool IsRewardCollected { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedDate{ get; set; }

    public int GameId { get; set; }
    public virtual Game Game { get; set; } = null!;

    public int UserChallengePeriodPerformanceId { get; set; }
    public virtual UserChallengePeriodPerformance UserChallengePeriodPerformance { get; set; } = null!;
}
