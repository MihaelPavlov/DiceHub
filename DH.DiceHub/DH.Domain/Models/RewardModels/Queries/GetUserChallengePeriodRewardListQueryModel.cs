using DH.Domain.Enums;

namespace DH.Domain.Models.RewardModels.Queries;

public class GetUserChallengePeriodRewardListQueryModel
{
    public int RewardImageId { get; set; }
    public int RewardRequiredPoints { get; set; }
    public bool IsCompleted { get; set; }
}
