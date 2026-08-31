namespace DH.Domain.Services;

public interface IUniversalChallengeProcessing
{
    Task ProcessUserChallengeTop3Streak(CancellationToken cancellationToken);

    /// <summary>
    /// Same as <see cref="ProcessUserChallengeTop3Streak(CancellationToken)"/> but scoped to a single
    /// tenant. Called by the per-tenant <c>UserChallengeTop3StreakTrackerJob</c> trigger, which fires
    /// at that tenant's configured local time.
    /// </summary>
    Task ProcessUserChallengeTop3Streak(string tenantId, CancellationToken cancellationToken);
    Task ProcessJoinXEventsChallenge(CancellationToken cancellationToken);
    Task<bool> PurchaseChallengeQrCodeProcessing(string userId, CancellationToken cancellationToken);
    Task<bool> UseXRewardsProcessing(string userId, CancellationToken cancellationToken);
}
