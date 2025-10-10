using DH.Domain.Entities;

namespace DH.Domain.Services;

public interface IRewardService : IDomainService<ChallengeReward>
{
    Task<int> CreateReward(ChallengeReward reward, string fileName, string contentType, MemoryStream imageStream, CancellationToken cancellationToken);
    Task UpdateReward(ChallengeReward reward, string fileName, string contentType, MemoryStream imageStream, CancellationToken cancellationToken);
    Task RewardConfirmation(int userChallengeRewardId, CancellationToken cancellationToken);
}
