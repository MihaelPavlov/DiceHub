using DH.Domain.Adapters.Scheduling;
using DH.Domain.Adapters.Scheduling.Enums;
using DH.Domain.Entities;
using DH.Domain.Repositories;

namespace DH.Adapter.Scheduling.Handlers;

/// <inheritdoc/>
internal class UserRewardsExpiryHandler : IUserRewardsExpiryHandler
{
    readonly IRepository<UserChallengeReward> repository;
    readonly IRepository<FailedJob> failedJobsRepository;

    public UserRewardsExpiryHandler(IRepository<UserChallengeReward> repository, IRepository<FailedJob> failedJobsRepository)
    {
        this.repository = repository;
        this.failedJobsRepository = failedJobsRepository;
    }

    /// <inheritdoc/>
    public async Task EvaluateUserRewardsExpiry(CancellationToken cancellationToken)
    {
        var rewardList = await this.repository.GetWithPropertiesAsTrackingAsync(
                   x => !x.IsClaimed || !x.IsExpired,
                   x => x,
                   cancellationToken);

        var currentDate = DateTime.UtcNow;

        foreach (var reward in rewardList)
        {
            if (currentDate >= reward.ExpiresDate)
                reward.IsExpired = true;
        }

        //TODO: Send notification to the user

        await this.repository.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task ProcessFailedExpiryCheck(string data, string errorMessage, CancellationToken cancellationToken)
    {
        await failedJobsRepository.AddAsync(new FailedJob { Data = data, Type = (int)JobType.UserRewardsExpiry, FailedAt = DateTime.UtcNow, ErrorMessage = errorMessage }, cancellationToken);
    }
}
