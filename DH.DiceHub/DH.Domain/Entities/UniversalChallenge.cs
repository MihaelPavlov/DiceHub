using DH.Domain.Enums;

namespace DH.Domain.Entities;

public class UniversalChallenge : TenantEntity
{
    public int Id { get; set; }
    public ChallengeRewardPoint RewardPoints { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
    public string Name_EN { get; set; } = string.Empty;
    public string Name_BG { get; set; } = string.Empty;
    public string Description_EN { get; set; } = string.Empty;
    public string Description_BG { get; set; } = string.Empty;
    public UniversalChallengeType Type { get; set; }
    public int Attempts { get; set; }

    public decimal? MinValue { get; set; }

    public virtual ICollection<UserChallenge> UserUniversalChallenges { get; set; } = [];
}