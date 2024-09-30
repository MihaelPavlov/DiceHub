using DH.Domain.Enums;

namespace DH.Domain.Entities;

public class UserChallenge
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int RequiredUserTotalPoints { get; set; }
    public ChallengeStatus Status { get; set; }
    public int AttemptCount { get; set; }
    public DateTime? CompletedDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public int ChallengeId { get; set; }

    public virtual Challenge Challenge { get; set; } = null!;
}
