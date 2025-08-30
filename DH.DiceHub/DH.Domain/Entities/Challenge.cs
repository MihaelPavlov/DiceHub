using DH.Domain.Enums;

namespace DH.Domain.Entities;

public class Challenge
{
    public int Id { get; set; }
    public ChallengeRewardPoint RewardPoints { get; set; }
    public int Attempts { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
    public int GameId { get; set; }

    public virtual Game Game { get; set; } = null!;
    public virtual ChallengeStatistic Statistic { get; set; } = null!;
    public virtual ICollection<UserChallenge> UserChallenges { get; set; } = [];
}
