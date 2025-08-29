using DH.Domain.Enums;

namespace DH.Domain.Models.ChallengeModels.Queries;

public class GetChallengeByIdQueryModel
{
    public int Id { get; set; }
    public ChallengeRewardPoint RewardPoints { get; set; }
    public int Attempts { get; set; }
    public ChallengeType Type { get; set; }
    public int GameId { get; set; }
}
