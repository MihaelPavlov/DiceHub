using DH.Domain.Enums;

namespace DH.Domain.Entities;

public class UserChallengePeriodPerformance
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public bool IsPeriodActive { get; set; }
    public int Points { get; set; }
    public int CompletedChallengeCount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public TimePeriodType TimePeriodType { get; set; }

    public virtual ICollection<UserChallengePeriodReward> UserChallengePeriodRewards { get; set; } = [];
}
