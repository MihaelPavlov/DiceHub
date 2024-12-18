using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Adapters.PushNotifications.Messages;
using DH.Domain.Adapters.Scheduling;
using DH.Domain.Adapters.Scheduling.Enums;
using DH.Domain.Entities;
using DH.Domain.Repositories;

namespace DH.Adapter.Scheduling.Handlers;

/// <summary>
/// Implementation of <see cref="IUserRewardsExpiryHandler"/>
/// </summary>
internal class UserRewardsExpiryHandler(IRepository<UserChallengeReward> repository, IRepository<FailedJob> failedJobsRepository, IPushNotificationsService pushNotificationsService) : IUserRewardsExpiryHandler
{
    readonly IRepository<UserChallengeReward> repository = repository;
    readonly IRepository<FailedJob> failedJobsRepository = failedJobsRepository;
    readonly IPushNotificationsService pushNotificationsService = pushNotificationsService;

    /// <inheritdoc/>
    public async Task EvaluateUserRewardsExpiry(CancellationToken cancellationToken)
    {
        var rewardList = await this.repository.GetWithPropertiesAsTrackingAsync(
                   x => !x.IsClaimed || !x.IsExpired,
                   x => x,
                   cancellationToken);

        var currentDate = DateTime.UtcNow;

        var expiredRewards = new List<ExpiredRewardInfo>();

        foreach (var reward in rewardList)
        {
            if (currentDate.Date >= reward.ExpiresDate.Date)
            {
                reward.IsExpired = true;
                expiredRewards.Add(new ExpiredRewardInfo(reward.UserId, reward.Reward.Name));
            }
        }

        await this.repository.SaveChangesAsync(cancellationToken);

        foreach (var reward in expiredRewards)
        {
            await this.pushNotificationsService.SendNotificationToUsersAsync(new List<Domain.Adapters.Authentication.Models.GetUserByRoleModel>
            {
                new()
                {
                    Id = reward.UserId,
                }
            }, new RewardExpiredMessage(reward.Name), cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task ProcessFailedExpiryCheck(string data, string errorMessage, CancellationToken cancellationToken)
    {
        await failedJobsRepository.AddAsync(new FailedJob { Data = data, Type = (int)JobType.UserRewardsExpiry, FailedAt = DateTime.UtcNow, ErrorMessage = errorMessage }, cancellationToken);
    }

    private record ExpiredRewardInfo(string UserId, string Name);
}
