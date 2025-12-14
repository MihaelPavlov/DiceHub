namespace DH.Domain.Entities;

public class CustomPeriodUniversalChallenge : TenantEntity
{
    public int Id { get; set; }
    public int Attempts { get; set; }
    public int RewardPoints { get; set; }
    public decimal? MinValue{ get; set; }

    public int UniversalChallengeId { get; set; }
    public virtual UniversalChallenge UniversalChallenge { get; set; } = null!;
}
