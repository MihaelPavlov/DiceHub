using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Adapters.PushNotifications.Messages;
using DH.Domain.Adapters.Scheduling;
using DH.Domain.Adapters.Scheduling.Enums;
using DH.Domain.Entities;
using DH.Domain.Helpers;
using DH.Domain.Repositories;
using DH.Domain.Services;
using DH.Domain.Services.TenantSettingsService;

namespace DH.Adapter.Scheduling.Handlers;

/// <summary>
/// Implementation of <see cref="IUserRewardsExpirationReminderHandler"/>
/// </summary>
internal class UserRewardsExpirationReminderHandler(
    IRepository<UserChallengeReward> repository, IRepository<FailedJob> failedJobsRepository,
    IPushNotificationsService pushNotificationsService, ITenantDirectoryService tenantDirectoryService,
    ITenantSettingsCacheService tenantSettingsCacheService,
    ITenantContextScopeRunner tenantContextScopeRunner) : IUserRewardsExpirationReminderHandler
{
    readonly IRepository<UserChallengeReward> repository = repository;
    readonly IRepository<FailedJob> failedJobsRepository = failedJobsRepository;
    readonly IPushNotificationsService pushNotificationsService = pushNotificationsService;
    readonly ITenantDirectoryService tenantDirectoryService = tenantDirectoryService;
    readonly ITenantSettingsCacheService tenantSettingsCacheService = tenantSettingsCacheService;
    readonly ITenantContextScopeRunner tenantContextScopeRunner = tenantContextScopeRunner;

    /// <inheritdoc/>
    public async Task EvaluateRewardsReminder(CancellationToken cancellationToken)
    {
        var tenantIds = await this.tenantDirectoryService.GetActiveTenantIdsAsync(cancellationToken);

        foreach (var tenantId in tenantIds)
            await EvaluateRewardsReminder(tenantId, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task EvaluateRewardsReminder(string tenantId, CancellationToken cancellationToken)
    {
        await this.tenantContextScopeRunner.RunAsTenantAsync(tenantId, async () =>
        {
            var tenantSettings = await this.tenantSettingsCacheService.GetGlobalTenantSettingsAsync(cancellationToken);
            var todayLocal = TimeZoneInfo
                .ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneResolver.Resolve(tenantSettings.TimeZoneId))
                .Date;

            var daysToRemind = new[] { 3, 2, 1 };

            foreach (var days in daysToRemind)
            {
                var reminderDate = todayLocal.AddDays(days);

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
            });
    }

    /// <inheritdoc/>
    public async Task ProcessFailedRewardExpirationReminder(string data, string errorMessage, CancellationToken cancellationToken)
    {
        await failedJobsRepository.AddAsync(new FailedJob { Data = data, Type = (int)JobType.UserRewardsExpiry, FailedAt = DateTime.UtcNow, ErrorMessage = errorMessage }, cancellationToken);
    }
}
