using DH.Domain.Enums;

namespace DH.Domain.Entities;

public class UserChallenge : TenantEntity
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    // RequiredUserTotalPoints, its only used inside the autogeneration of challenges, to control do logic which challenges are in progress and which locked
    // But i have thoughts that we might not use this at all, because we woudn't need to know when to unlocked a locked challenge, because will happen during the AssignNextChallengeToUserAsync
    public ChallengeRewardPoint RequiredUserTotalPoints { get; set; }
    public ChallengeStatus Status { get; set; }
    public bool IsActive { get; set; }
    public bool IsRewardCollected { get; set; }
    public int AttemptCount { get; set; }
    public DateTime? CompletedDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public int? ChallengeId { get; set; }
    public int? UniversalChallengeId { get; set; }

    // For Game Challenge
    public virtual Challenge? Challenge { get; set; } = null!;

    // For Universal Challenge Type Play User Favorite Game
    public int? GameId { get; set; }
    public virtual Game? Game { get; set; }
    // For Universal Challenge
    public virtual UniversalChallenge? UniversalChallenge { get; set; } = null!;
}
