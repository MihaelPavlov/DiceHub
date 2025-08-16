using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Adapters.PushNotifications.Messages;
using DH.Domain.Adapters.Scheduling;
using DH.Domain.Adapters.Scheduling.Enums;
using DH.Domain.Adapters.Statistics;
using DH.Domain.Adapters.Statistics.Services;
using DH.Domain.Entities;
using DH.Domain.Repositories;

namespace DH.Adapter.Scheduling.Handlers;

/// <summary>
/// Implementation of <see cref="IUserRewardsExpiryHandler"/>
/// </summary>
internal class UserRewardsExpiryHandler(IRepository<UserChallengeReward> repository, IRepository<ChallengeReward> userChallengeRewardsRepository, IStatisticQueuePublisher statisticQueuePublisher, IRepository<FailedJob> failedJobsRepository, IPushNotificationsService pushNotificationsService) : IUserRewardsExpiryHandler
{
    readonly IRepository<UserChallengeReward> repository = repository;
    readonly IRepository<ChallengeReward> userChallengeRewardsRepository = userChallengeRewardsRepository;
    readonly IRepository<FailedJob> failedJobsRepository = failedJobsRepository;
    readonly IPushNotificationsService pushNotificationsService = pushNotificationsService;
    readonly IStatisticQueuePublisher statisticQueuePublisher = statisticQueuePublisher;

    /// <inheritdoc/>
    public async Task EvaluateUserRewardsExpiry(CancellationToken cancellationToken)
    {
        var rewardList = await this.repository.GetWithPropertiesAsTrackingAsync(
            x => !x.IsClaimed && !x.IsExpired && DateTime.UtcNow.Date >= x.ExpiresDate.Date,
            x => x,
            cancellationToken);

        if (rewardList.Count == 0)
            return;

        var rewardIds = rewardList.Select(x => x.RewardId).ToList();
        var rewards = await this.userChallengeRewardsRepository.GetWithPropertiesAsync(x => rewardIds.Contains(x.Id), x => x, cancellationToken);

        var currentDate = DateTime.UtcNow;

        var expiredRewards = new List<ExpiredRewardInfo>();

        foreach (var reward in rewardList)
        {
            var dbReward = rewards.First(x => x.Id == reward.RewardId);
            if (currentDate.Date >= reward.ExpiresDate.Date)
            {
                reward.IsExpired = true;
                expiredRewards.Add(new ExpiredRewardInfo(reward.UserId, reward.RewardId, dbReward.Name, reward.ExpiresDate));
            }
        }

        await this.repository.SaveChangesAsync(cancellationToken);

        foreach (var reward in expiredRewards)
        {
            await this.statisticQueuePublisher.PublishAsync(
                new StatisticJobQueue.RewardActionDetectedJob(
                    reward.UserId, reward.RewardId,
                    CollectedDate: null, ExpiredDate: reward.ExpiredDate,
                    IsExpired: true, IsCollected: false));

            var payload = new RewardExpiredNotification { RewardName = reward.Name };

            await this.pushNotificationsService
                .SendNotificationToUsersAsync([reward.UserId], payload, cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task ProcessFailedExpiryCheck(string data, string errorMessage, CancellationToken cancellationToken)
    {
        await failedJobsRepository.AddAsync(new FailedJob { Data = data, Type = (int)JobType.UserRewardsExpiry, FailedAt = DateTime.UtcNow, ErrorMessage = errorMessage }, cancellationToken);
    }

    private record ExpiredRewardInfo(string UserId, int RewardId, string Name, DateTime ExpiredDate);
}
