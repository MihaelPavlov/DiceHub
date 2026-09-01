namespace DH.Domain.Entities;

public class ChallengeStatistic : TenantEntity
{
    public int Id { get; set; }
    public int TotalCompletions { get; set; }
    public int ChallengeId { get; set; }

    public virtual Challenge Challenge { get; set; } = null!;
}
