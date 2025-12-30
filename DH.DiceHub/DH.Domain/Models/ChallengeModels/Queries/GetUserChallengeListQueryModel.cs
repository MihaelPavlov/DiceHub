using DH.Domain.Enums;

namespace DH.Domain.Models.ChallengeModels.Queries;

public class GetUserChallengeListQueryModel
{
    public int Id { get; set; }
    public ChallengeRewardPoint RewardPoints { get; set; }
    public int MaxAttempts { get; set; }
    public int CurrentAttempts { get; set; }
    public ChallengeStatus Status { get; set; }
    public string GameImageUrl { get; set; } = string.Empty;
    public string GameName { get; set; } = string.Empty;
}
