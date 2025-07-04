namespace DH.Domain.Models.ChallengeModels.Queries;

public class GetUserCustomPeriodQueryModel
{
    public List<GetUserCustomPeriodRewardQueryModel> Rewards { get; set; } = [];
    public List<GetUserCustomPeriodChallengeQueryModel> Challenges { get; set; } = [];
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
