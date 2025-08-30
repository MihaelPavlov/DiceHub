using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Adapters.PushNotifications.Messages;
using DH.Domain.Adapters.Scheduling;
using DH.Domain.Adapters.Scheduling.Enums;
using DH.Domain.Entities;
using DH.Domain.Repositories;

namespace DH.Adapter.Scheduling.Handlers;

/// <summary>
/// Implementation of <see cref="IUserRewardsExpirationReminderHandler"/>
/// </summary>
internal class UserRewardsExpirationReminderHandler(
    IRepository<UserChallengeReward> repository, IRepository<FailedJob> failedJobsRepository,
    IPushNotificationsService pushNotificationsService) : IUserRewardsExpirationReminderHandler
{
    readonly IRepository<UserChallengeReward> repository = repository;
    readonly IRepository<FailedJob> failedJobsRepository = failedJobsRepository;
    readonly IPushNotificationsService pushNotificationsService = pushNotificationsService;

    /// <inheritdoc/>
    public async Task EvaluateRewardsReminder(CancellationToken cancellationToken)
    {
        var daysToRemind = new[] { 3, 2, 1 };

        foreach (var days in daysToRemind)
        {
            var reminderDate = DateTime.UtcNow.Date.AddDays(days);

            var rewards = await this.repository.GetWithPropertiesAsTrackingAsync(
                x => x.ExpiresDate.Date == reminderDate && !x.IsExpired && !x.IsClaimed,
                x => x,
                cancellationToken);

            foreach (var reward in rewards)
            {
                try
                {
                    var payload = new RewardExpirationReminderNotification
                    {
                        RewardName_EN = reward.Reward.Name_EN,
                        RewardName_BG = reward.Reward.Name_BG,
                        Days = days
                    };

                    await this.pushNotificationsService
                        .SendNotificationToUsersAsync([reward.UserId], payload, cancellationToken);
                }
                catch (Exception ex)
                {
                    await ProcessFailedRewardExpirationReminder(
                        $"RewardId: {reward.RewardId}, UserId: {reward.UserId}, ReminderDays: {days}",
                        ex.Message,
                        cancellationToken);
                }
            }
        }
    }

    /// <inheritdoc/>
    public async Task ProcessFailedRewardExpirationReminder(string data, string errorMessage, CancellationToken cancellationToken)
    {
        await failedJobsRepository.AddAsync(new FailedJob { Data = data, Type = (int)JobType.UserRewardsExpiry, FailedAt = DateTime.UtcNow, ErrorMessage = errorMessage }, cancellationToken);
    }
}
