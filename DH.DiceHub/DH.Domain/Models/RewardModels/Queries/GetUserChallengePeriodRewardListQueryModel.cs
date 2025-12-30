namespace DH.Domain.Models.RewardModels.Queries;

public class GetUserChallengePeriodRewardListQueryModel
{
    public string RewardImageUrl { get; set; } = string.Empty;
    public int RewardRequiredPoints { get; set; }
    public bool IsCompleted { get; set; }
}
