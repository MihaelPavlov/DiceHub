namespace DH.Domain.Services;

public interface IUniversalChallengeProcessing
{
    Task ProcessUserChallengeTop3Streak(CancellationToken cancellationToken);
    Task<bool> PurchaseChallengeQrCodeProcessing(string userId, CancellationToken cancellationToken);
    Task<bool> UseXRewardsProcessing(string userId, CancellationToken cancellationToken);
}
