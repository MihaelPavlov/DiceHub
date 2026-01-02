using DH.Domain.Enums;

namespace DH.Domain.Models.ChallengeModels.Queries;

public class GetChallengeListWithFilterQueryModel
{
    public int Id { get; set; }
    public ChallengeRewardPoint RewardPoints { get; set; }
    public int Attempts { get; set; }
    public string GameImageUrl { get; set; } = string.Empty;
}
