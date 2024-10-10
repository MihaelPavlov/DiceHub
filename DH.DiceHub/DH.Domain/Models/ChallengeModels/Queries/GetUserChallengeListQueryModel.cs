using DH.Domain.Enums;

namespace DH.Domain.Models.ChallengeModels.Queries;

public class GetUserChallengeListQueryModel
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public ChallengeRewardPoint RewardPoints { get; set; }
    public int MaxAttempts { get; set; }
    public int CurrentAttempts { get; set; }
    public ChallengeStatus Status { get; set; }
    public int GameImageId { get; set; }
    public string GameName { get; set; } = string.Empty;
}
