using DH.Domain.Adapters.ChallengeHub;
using DH.Domain.Adapters.Statistics;
using DH.Domain.Adapters.Statistics.Enums;
using DH.Domain.Adapters.Statistics.Services;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Extensions;
using DH.Domain.Helpers;
using DH.Domain.Services;
using DH.Domain.Services.TenantSettingsService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace DH.Adapter.Data.Services;

internal class UniversalChallengeProcessing(
    IDbContextFactory<TenantDbContext> dbContextFactory,
    ITenantSettingsCacheService tenantSettingsCacheService,
    ILogger<UniversalChallengeProcessing> logger,
    IChallengeHubClient challengeHubClient,
    IStatisticQueuePublisher statisticQueuePublisher,
    IStatisticsService statisticsService) : IUniversalChallengeProcessing
{
    readonly IDbContextFactory<TenantDbContext> dbContextFactory = dbContextFactory;
    readonly ITenantSettingsCacheService tenantSettingsCacheService = tenantSettingsCacheService;
    readonly ILogger<UniversalChallengeProcessing> logger = logger;
    readonly IChallengeHubClient challengeHubClient = challengeHubClient;
    readonly IStatisticQueuePublisher statisticQueuePublisher = statisticQueuePublisher;
    readonly IStatisticsService statisticsService = statisticsService;

    public async Task ProcessUserChallengeTop3Streak(CancellationToken cancellationToken)
    {
        var tenantSettings = await this.tenantSettingsCacheService.GetGlobalTenantSettingsAsync(cancellationToken);

        if (tenantSettings.DaysOff.IsTodayDayOff())
        {
            this.logger.LogInformation("Skipping Universal Challenge Top 3 Users Processing - Today is day off.");
            return;
        }

        var challengeHistory = await this.statisticsService
            .GetChallengeHistoryLogs(ChallengeHistoryLogType.Weekly, cancellationToken);

        //  purpose : Maintain a top 3 position on the overall Challenge Leaderboard for consecutive days. Progress is tracked daily. Field Attempts represent days for this challenge",
        if (challengeHistory.Success && challengeHistory.RelatedObject != null)
        {
            var top3Users = challengeHistory.RelatedObject.Take(3).Select(u => u.UserId).ToList();


            using (var context = await this.dbContextFactory.CreateDbContextAsync(cancellationToken))
            {
                using (var transaction = await context.Database.BeginTransactionAsync(cancellationToken))
                {
                    if (tenantSettings.IsCustomPeriodOn)
                    {
                        var activePeriods = await context.UserChallengePeriodPerformances
                            .Include(x => x.CustomPeriodUserUniversalChallenges)
                            .ThenInclude(x => x.UniversalChallenge)
                            .Where(x => x.IsPeriodActive)
                            .ToListAsync(cancellationToken);

                        foreach (var period in activePeriods)
                        {
                            var userId = period.UserId;
                            var top3Challenge = period.CustomPeriodUserUniversalChallenges
                                .FirstOrDefault(x => x.UniversalChallenge.Type == UniversalChallengeType.Top3ChallengeLeaderboard);

                            if (top3Challenge == null || top3Challenge.IsCompleted)
                                continue;

                            if (top3Users.Contains(userId))
                            {
                                // user is still in top 3, continue streak
                                top3Challenge.UserAttempts++;

                                if (top3Challenge.UserAttempts >= top3Challenge.ChallengeAttempts && !top3Challenge.IsCompleted)
                                {
                                    top3Challenge.IsCompleted = true;
                                    top3Challenge.CompletedDate = DateTime.UtcNow;
                                    top3Challenge.IsRewardCollected = true;

                                    period.Points += top3Challenge.RewardPoints;

                                    await this.challengeHubClient.SendUniversalChallengeCompleted(
                                        userId,
                                        top3Challenge.UniversalChallenge!.Name_EN,
                                        top3Challenge.UniversalChallenge!.Name_BG,
                                        top3Challenge.RewardPoints);
                                }
                                else
                                {
                                    await this.challengeHubClient.SendUniversalChallengeUpdated(
                                        userId,
                                        top3Challenge.UniversalChallenge!.Name_EN,
                                        top3Challenge.UniversalChallenge!.Name_BG);
                                }
                            }
                            else
                            {
                                if (top3Challenge.UserAttempts > 0)
                                {
                                    top3Challenge.UserAttempts = 0;

                                    await this.challengeHubClient.SendUniversalChallengeRestarted(
                                        userId,
                                        top3Challenge.UniversalChallenge!.Name_EN,
                                        top3Challenge.UniversalChallenge!.Name_BG);
                                }
                            }
                        }
                    }
                    else
                    {
                        var periods = await context.UserChallengePeriodPerformances
                            .AsTracking()
                            .Include(x => x.UserChallengePeriodRewards)
                                .ThenInclude(r => r.ChallengeReward)
                            .Where(x => x.IsPeriodActive)
                            .ToListAsync(cancellationToken);

                        foreach (var period in periods)
                        {
                            var userId = period.UserId;

                            var userUniversalChallenges = await context.UserChallenges
                            .AsTracking()
                            .Include(x => x.UniversalChallenge)
                            .Where(x =>
                                x.UserId == userId &&
                                x.IsActive &&
                                x.Status == ChallengeStatus.InProgress &&
                                x.UniversalChallenge != null)
                            .ToListAsync(cancellationToken);

                            var top3Challenge = userUniversalChallenges
                                .FirstOrDefault(x => x.UniversalChallenge!.Type == UniversalChallengeType.Top3ChallengeLeaderboard);

                            if (top3Challenge == null || top3Challenge.IsActive)
                                continue;
                            if (top3Users.Contains(userId))
                            {
                                top3Challenge.AttemptCount++;

                                if (top3Challenge.AttemptCount >= top3Challenge.UniversalChallenge!.Attempts)
                                {
                                    top3Challenge.Status = ChallengeStatus.Completed;
                                    top3Challenge.CompletedDate = DateTime.UtcNow;
                                    top3Challenge.IsActive = false;
                                    top3Challenge.IsRewardCollected = true;

                                    await this.challengeHubClient.SendUniversalChallengeCompleted(
                                        userId,
                                        top3Challenge.UniversalChallenge!.Name_EN,
                                        top3Challenge.UniversalChallenge!.Name_BG,
                                        (int)top3Challenge.UniversalChallenge!.RewardPoints);
                                }
                                else
                                {
                                    await this.challengeHubClient.SendUniversalChallengeUpdated(
                                        userId,
                                        top3Challenge.UniversalChallenge!.Name_EN,
                                        top3Challenge.UniversalChallenge!.Name_BG);
                                }
                            }
                            else
                            {
                                if (top3Challenge.AttemptCount > 0)
                                {
                                    top3Challenge.AttemptCount = 0;
                                    await this.challengeHubClient.SendUniversalChallengeRestarted(
                                        userId,
                                        top3Challenge.UniversalChallenge!.Name_EN,
                                        top3Challenge.UniversalChallenge!.Name_BG);
                                }
                            }
                        }
                    }

                    await context.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                }
            }
        }
    }

    public async Task<bool> PurchaseChallengeQrCodeProcessing(string userId, CancellationToken cancellationToken)
    {
        return await ProcessUniversalChallengeByType(userId, UniversalChallengeType.BuyItems, cancellationToken);
    }

    public async Task<bool> UseXRewardsProcessing(string userId, CancellationToken cancellationToken)
    {
        return await ProcessUniversalChallengeByType(userId, UniversalChallengeType.UseRewards, cancellationToken);
    }

    private async Task<bool> ProcessUniversalChallengeByType(string userId, UniversalChallengeType universalChallengeType, CancellationToken cancellationToken)
    {
        var tenantSettings = await this.tenantSettingsCacheService.GetGlobalTenantSettingsAsync(cancellationToken);

        using (var context = await this.dbContextFactory.CreateDbContextAsync(cancellationToken))
        {
            using (var transaction = await context.Database.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    if (tenantSettings.IsCustomPeriodOn)
                    {
                        await ProcessUniversalChallengeForCustomPeriod(
                            userId, universalChallengeType,
                            tenantSettings, context, cancellationToken);
                    }
                    else
                    {
                        await ProcessUniversallChallengeForUserPeriod(
                            userId, universalChallengeType,
                            tenantSettings, context, cancellationToken);
                    }

                    await context.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    this.logger.LogError(ex,
                        "Error during UniversalChallengeProcessing.ProcessUniversalChallengeByType Type {Type} and UserId {UserId}. Exception: {Message}",
                        UniversalChallengeType.BuyItems.ToString(),
                        userId,
                        ex.Message);
                    return false;
                }
            }
        }

        return true;
    }

    private async Task ProcessUniversallChallengeForUserPeriod(
        string userId, UniversalChallengeType universalChallengeType,
        TenantSetting tenantSettings, TenantDbContext context, CancellationToken cancellationToken)
    {
        var periods = await context.UserChallengePeriodPerformances
            .AsTracking()
            .Include(x => x.UserChallengePeriodRewards)
                .ThenInclude(r => r.ChallengeReward)
            .Where(x => x.UserId == userId && x.IsPeriodActive)
            .ToListAsync(cancellationToken);

        var activePeriod = periods.GetActiveUserPeriod(this.logger, userId);
        if (activePeriod == null)
            return;

        var userUniversalChallenges = await context.UserChallenges
            .AsTracking()
            .Include(x => x.UniversalChallenge)
            .Where(x =>
                x.UserId == userId &&
                x.IsActive &&
                x.Status == ChallengeStatus.InProgress &&
                x.UniversalChallenge != null)
            .ToListAsync(cancellationToken);

        var selectedUniversalChallenge = userUniversalChallenges
            .FirstOrDefault(x => x.UniversalChallenge!.Type == universalChallengeType);

        if (selectedUniversalChallenge != null)
        {
            selectedUniversalChallenge.AttemptCount++;

            if (selectedUniversalChallenge.UniversalChallenge!.Attempts == selectedUniversalChallenge.AttemptCount)
            {
                selectedUniversalChallenge.IsActive = false;
                selectedUniversalChallenge.CompletedDate = DateTime.UtcNow;
                selectedUniversalChallenge.IsRewardCollected = true;
                selectedUniversalChallenge.Status = ChallengeStatus.Completed;

                activePeriod.Points += (int)selectedUniversalChallenge.UniversalChallenge.RewardPoints;

                await this.challengeHubClient.SendUniversalChallengeCompleted(
                    userId,
                    selectedUniversalChallenge.UniversalChallenge!.Name_EN,
                    selectedUniversalChallenge.UniversalChallenge!.Name_BG,
                    (int)selectedUniversalChallenge.UniversalChallenge.RewardPoints);

                await this.statisticQueuePublisher.PublishAsync(new ChallengeProcessingOutcomeJob(
                    userId,
                    100000 + selectedUniversalChallenge.UniversalChallengeId!.Value,
                    ChallengeOutcome.Completed,
                    selectedUniversalChallenge.CompletedDate.Value,
                    DateTime.UtcNow));

                foreach (var userPeriodReward in activePeriod.UserChallengePeriodRewards.Where(x => !x.IsCompleted))
                {
                    if (activePeriod.Points >= (int)userPeriodReward.ChallengeReward.RequiredPoints)
                    {
                        userPeriodReward.IsCompleted = true;
                        await context.UserChallengeRewards.AddAsync(new UserChallengeReward
                        {
                            UserId = userId,
                            AvailableFromDate = DateTime.UtcNow,
                            ExpiresDate = DateTime.UtcNow.AddDays(Enum.Parse<TimePeriodType>(tenantSettings.PeriodOfRewardReset).GetDays()),
                            IsClaimed = false,
                            RewardId = userPeriodReward.ChallengeRewardId,
                            IsExpired = false,
                        }, cancellationToken);

                        await this.challengeHubClient.SendRewardGranted(
                            userId,
                            userPeriodReward.ChallengeReward.Name_BG,
                            userPeriodReward.ChallengeReward.Name_EN);
                    }
                }
            }
            else
            {
                await this.challengeHubClient.SendUniversalChallengeUpdated(
                    userId,
                    selectedUniversalChallenge.UniversalChallenge!.Name_EN,
                    selectedUniversalChallenge.UniversalChallenge!.Name_BG);
            }
        }
    }

    private async Task ProcessUniversalChallengeForCustomPeriod(
        string userId, UniversalChallengeType universalChallengeType, TenantSetting tenantSettings,
        TenantDbContext context, CancellationToken cancellationToken)
    {
        var customPeriods = await context.UserChallengePeriodPerformances
            .AsTracking()
            .Include(x => x.CustomPeriodUserChallenges)
            .ThenInclude(x => x.Game)
            .Include(x => x.CustomPeriodUserRewards)
            .ThenInclude(x => x.Reward)
            .Include(x => x.CustomPeriodUserUniversalChallenges)
            .ThenInclude(x => x.UniversalChallenge)
            .Include(x => x.CustomPeriodUserUniversalChallenges)
            .ThenInclude(x => x.Game)
            .Where(x => x.UserId == userId && x.IsPeriodActive)
            .ToListAsync(cancellationToken);

        var customPeriod = customPeriods.GetActiveUserCustomPeriod(this.logger, userId);
        if (customPeriod == null)
            return;

        var selectedUniversalChallenge = customPeriod.CustomPeriodUserUniversalChallenges
            .FirstOrDefault(x => x.UniversalChallenge.Type == universalChallengeType);

        if (selectedUniversalChallenge != null && !selectedUniversalChallenge.IsCompleted)
        {
            selectedUniversalChallenge.UserAttempts++;

            if (selectedUniversalChallenge.UserAttempts == selectedUniversalChallenge.ChallengeAttempts)
            {
                selectedUniversalChallenge.IsCompleted = true;
                selectedUniversalChallenge.CompletedDate = DateTime.UtcNow;
                selectedUniversalChallenge.IsRewardCollected = true;

                customPeriod.Points += selectedUniversalChallenge.RewardPoints;

                await this.challengeHubClient.SendUniversalChallengeCompleted(
                    userId,
                    selectedUniversalChallenge.UniversalChallenge!.Name_EN,
                    selectedUniversalChallenge.UniversalChallenge!.Name_BG,
                    selectedUniversalChallenge.RewardPoints);

                await this.statisticQueuePublisher.PublishAsync(new ChallengeProcessingOutcomeJob(
                    userId,
                    100000 + selectedUniversalChallenge.UniversalChallengeId,
                    ChallengeOutcome.Completed,
                    selectedUniversalChallenge.CompletedDate.Value,
                    DateTime.UtcNow));

                foreach (var userCustomPeriodReward in customPeriod.CustomPeriodUserRewards.Where(x => !x.IsCompleted))
                {
                    if (customPeriod.Points >= userCustomPeriodReward.RequiredPoints)
                    {
                        userCustomPeriodReward.IsCompleted = true;
                        await context.UserChallengeRewards.AddAsync(new UserChallengeReward
                        {
                            UserId = userId,
                            AvailableFromDate = DateTime.UtcNow,
                            ExpiresDate = DateTime.UtcNow.AddDays(Enum.Parse<TimePeriodType>(tenantSettings.PeriodOfRewardReset).GetDays()),
                            IsClaimed = false,
                            RewardId = userCustomPeriodReward.RewardId,
                            IsExpired = false,
                        }, cancellationToken);

                        await this.challengeHubClient.SendRewardGranted(
                            userId,
                            userCustomPeriodReward.Reward.Name_BG,
                            userCustomPeriodReward.Reward.Name_EN);
                    }
                }
            }
            else
            {
                await this.challengeHubClient.SendUniversalChallengeUpdated(
                    userId,
                    selectedUniversalChallenge.UniversalChallenge!.Name_EN,
                    selectedUniversalChallenge.UniversalChallenge!.Name_BG);
            }
        }
    }

    public async Task ProcessJoinXEventsChallenge(CancellationToken cancellationToken)
    {
        using (var context = await this.dbContextFactory.CreateDbContextAsync(cancellationToken))
        {
            var events = await context.Events
                .Where(x => !x.IsDeleted && !x.IsJoinChallengeProcessed && x.StartDate < DateTime.UtcNow)
                .Include(x => x.Participants)
                .AsSplitQuery()
                .ToListAsync(cancellationToken);

            foreach (var currentEvent in events)
            {
                foreach (var participant in currentEvent.Participants)
                {
                    await ProcessUniversalChallengeByType(
                        participant.UserId,
                        UniversalChallengeType.JoinEvents,
                        cancellationToken);
                }
                currentEvent.IsJoinChallengeProcessed = true;
            }

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
