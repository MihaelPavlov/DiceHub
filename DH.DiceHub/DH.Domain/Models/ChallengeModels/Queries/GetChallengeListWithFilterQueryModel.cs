using DH.Domain.Enums;

namespace DH.Domain.Models.ChallengeModels.Queries;

public class GetChallengeListWithFilterQueryModel
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public ChallengeRewardPoint RewardPoints { get; set; }
    public int Attempts { get; set; }
    public int GameId { get; set; }
}
