using DH.Domain.Enums;

namespace DH.Domain.Models.ChallengeModels.Queries;

public class GetUserUniversalChallengeListQueryModel
{
    public int Id { get; set; }
    public ChallengeRewardPoint RewardPoints { get; set; }
    public int MaxAttempts { get; set; }
    public int CurrentAttempts { get; set; }
    public ChallengeStatus Status { get; set; }
    public UniversalChallengeType Type { get; set; }

    public decimal? MinValue { get; set; }
    public int? GameImageId { get; set; }
    public string? GameName { get; set; } = string.Empty;

    public string Name_EN { get; set; } = string.Empty;
    public string Name_BG { get; set; } = string.Empty;
    public string Description_EN { get; set; } = string.Empty;
    public string Description_BG { get; set; } = string.Empty;
}