using DH.Domain.Enums;

namespace DH.Domain.Entities;

public class UserChallengePeriodPerformance : TenantEntity
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public bool IsPeriodActive { get; set; }
    public int Points { get; set; }
    public int CompletedChallengeCount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public TimePeriodType TimePeriodType { get; set; }

    public virtual ICollection<UserChallengePeriodReward> UserChallengePeriodRewards { get; set; } = [];

    public virtual ICollection<CustomPeriodUserChallenge> CustomPeriodUserChallenges { get; set; } = [];
    public virtual ICollection<CustomPeriodUserUniversalChallenge> CustomPeriodUserUniversalChallenges { get; set; } = [];
    public virtual ICollection<CustomPeriodUserReward> CustomPeriodUserRewards { get; set; } = [];
}
