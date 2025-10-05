using DH.Domain.Enums;

namespace DH.Domain.Models.ChallengeModels.Queries;

public class GetUserCustomPeriodQueryModel
{
    public List<GetUserCustomPeriodRewardQueryModel> Rewards { get; set; } = [];
    public List<GetUserCustomPeriodChallengeQueryModel> Challenges { get; set; } = [];
    public List<GetUserCustomPeriodUniversalChallengeQueryModel> UniversalChallenges { get; set; } = [];
}

public class GetUserCustomPeriodRewardQueryModel
{
    public int RewardImageId { get; set; }
    public int RewardRequiredPoints { get; set; }
    public bool IsCompleted { get; set; }
}

public class GetUserCustomPeriodChallengeQueryModel
{
    public bool IsCompleted { get; set; }
    public int ChallengeAttempts { get; set; }
    public int CurrentAttempts { get; set; }
    public int RewardPoints { get; set; }
    public int GameImageId { get; set; }
    public string GameName { get; set; } = string.Empty;
}

public class GetUserCustomPeriodUniversalChallengeQueryModel
{
    public int MaxAttempts { get; set; }
    public int CurrentAttempts { get; set; }
    public int RewardPoints { get; set; }
    public ChallengeStatus Status { get; set; }
    public UniversalChallengeType Type { get; set; }

    // For Game Challenges
    public decimal? MinValue { get; set; }
    public int? GameImageId { get; set; }
    public string? GameName { get; set; }

    // For Universal Challenges
    public string Name_EN { get; set; } = string.Empty;
    public string Name_BG { get; set; } = string.Empty;
    public string Description_EN { get; set; } = string.Empty;
    public string Description_BG { get; set; } = string.Empty;
}
