namespace DH.Domain.Services;

public interface IUniversalChallengeProcessing
{
    Task<bool> PurchaseChallengeQrCodeProcessing(string userId, CancellationToken cancellationToken);
    Task<bool> UseXRewardsProcessing(string userId, CancellationToken cancellationToken);
}
