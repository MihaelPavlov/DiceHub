using DH.Domain.Adapters.Authentication.Models.Enums;
using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.ChallengesOrchestrator;
using DH.Domain.Adapters.Statistics;
using DH.Domain.Adapters.Statistics.Services;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Helpers;
using DH.Domain.Services;
using DH.Domain.Services.TenantSettingsService;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace DH.Adapter.Data.Services;

/// <inheritdoc/>
public class GameSessionService : IGameSessionService
{
    readonly IDbContextFactory<TenantDbContext> dbContextFactory;
    readonly SynchronizeUsersChallengesQueue queue;
    readonly ITenantSettingsCacheService tenantSettingsCacheService;
    readonly IStatisticQueuePublisher statisticQueuePublisher;
    readonly IUserService userService;
    readonly ILogger<GameSessionService> logger;

    public GameSessionService(
        IDbContextFactory<TenantDbContext> dbContextFactory,
        SynchronizeUsersChallengesQueue queue,
        ITenantSettingsCacheService tenantSettingsCacheService,
        IStatisticQueuePublisher statisticQueuePublisher,
        IUserService userService,
        ILogger<GameSessionService> logger)
    {
        this.dbContextFactory = dbContextFactory;
        this.queue = queue;
        this.tenantSettingsCacheService = tenantSettingsCacheService;
        this.statisticQueuePublisher = statisticQueuePublisher;
        this.userService = userService;
        this.logger = logger;
    }

    /// <inheritdoc/>
    public async Task<bool> ProcessChallengeAfterSession(string userId, int gameId, CancellationToken cancellationToken)
    {
        using (var context = await this.dbContextFactory.CreateDbContextAsync(cancellationToken))
        {
            using (var transaction = await context.Database.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    var tenantSettings = await this.tenantSettingsCacheService.GetGlobalTenantSettingsAsync(cancellationToken);

                    if (tenantSettings.IsCustomPeriodOn)
                    {
                        var customPeriod = await TryGetActiveCustomPeriodAsync(context, userId, cancellationToken);
                        if (customPeriod == null)
                            return false;

                        var customPeriodUserChallenges = customPeriod.CustomPeriodUserChallenges.Where(x => !x.IsCompleted && x.GameId == gameId);

                        if (!customPeriodUserChallenges.Any())
                            return false;

                        foreach (var customPeriodUserChallenge in customPeriodUserChallenges)
                        {
                            var currentAttempts = customPeriodUserChallenge.UserAttempts;
                            var challengeAttempts = customPeriodUserChallenge.ChallengeAttempts;

                            if (currentAttempts < challengeAttempts)
                            {
                                customPeriodUserChallenge.UserAttempts++;
                            }
                        }

                        var completedCustomPeriodUserChallenges = customPeriodUserChallenges.Where(x => x.UserAttempts == x.ChallengeAttempts);

                        foreach (var customPeriodUserChallenge in completedCustomPeriodUserChallenges)
                        {
                            customPeriodUserChallenge.CompletedDate = DateTime.UtcNow;
                            customPeriodUserChallenge.IsCompleted = true;

                            if (!await this.userService.HasUserAnyMatchingRole(userId, Role.SuperAdmin))
                            {
                                await this.statisticQueuePublisher.PublishAsync(new StatisticJobQueue.ChallengeProcessingOutcomeJob(
                                    userId,
                                    10000 + customPeriodUserChallenge.Id, // Number the indicated the custom challenges
                                    ChallengeOutcome.Completed,
                                    customPeriodUserChallenge.CompletedDate.Value,
                                    DateTime.UtcNow));
                            }
                        }
                    }
                    else
                    {
                        var userChallenges = await context.UserChallenges
                            .AsTracking()
                            .Include(x => x.Challenge)
                            .Where(x =>
                                x.UserId == userId &&
                                x.IsActive &&
                                x.Status == ChallengeStatus.InProgress &&
                                gameId == x.Challenge.GameId)
                            .ToListAsync(cancellationToken);

                        if (!userChallenges.Any())
                            return false;

                        foreach (var userChallenge in userChallenges)
                        {
                            var currentAttempts = userChallenge.AttemptCount;
                            var challengeAttempts = userChallenge.Challenge.Attempts;

                            if (currentAttempts < challengeAttempts)
                            {
                                userChallenge.AttemptCount++;
                            }
                        }

                        var completedChallenges = userChallenges.Where(x => x.AttemptCount == x.Challenge.Attempts);

                        if (!completedChallenges.Any())
                        {
                            await SaveAndCommitTransaction(context, transaction, cancellationToken);

                            return false;
                        }

                        var challengeStatistics = await context.ChallengeStatistics
                            .AsTracking()
                            .Where(x =>
                                completedChallenges.Select(c => c.ChallengeId).Contains(x.ChallengeId))
                            .ToListAsync(cancellationToken);

                        foreach (var challenge in completedChallenges)
                        {
                            challenge.CompletedDate = DateTime.UtcNow;
                            challenge.Status = ChallengeStatus.Completed;
                            challenge.IsActive = false;

                            if (!await this.userService.HasUserAnyMatchingRole(userId, Role.SuperAdmin))
                            {
                                await this.statisticQueuePublisher.PublishAsync(new StatisticJobQueue.ChallengeProcessingOutcomeJob(
                                    userId,
                                    challenge.ChallengeId,
                                    ChallengeOutcome.Completed,
                                    challenge.CompletedDate.Value,
                                    DateTime.UtcNow));
                            }

                            var challengeStats = challengeStatistics.First(x => x.ChallengeId == challenge.ChallengeId);
                            challengeStats.TotalCompletions++;
                        }

                        var lockedChallenges = await context.UserChallenges
                            .Where(uc => uc.UserId == userId && uc.Status == ChallengeStatus.Locked)
                            .ToListAsync(cancellationToken);

                        if (lockedChallenges.Count != 0)
                            this.queue.AddChallengeInitiationJob(userId, DateTime.UtcNow);
                        else
                        {
                            this.queue.AddChallengeInitiationJob(userId, DateTime.UtcNow.AddHours(tenantSettings.ChallengeInitiationDelayHours));
                        }
                    }

                    await SaveAndCommitTransaction(context, transaction, cancellationToken);

                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    this.logger.LogError(ex,
                        "Error during GameSessionService.ProcessChallengeAfterSession for UserId {UserId}. Exception: {Message}",
                        userId,
                        ex.Message);
                    return false;
                }
            }
        }
    }

    /// <inheritdoc/>
    public async Task<bool> CollectRewardsFromChallenges(string userId, CancellationToken cancellationToken)
    {
        using (var context = await this.dbContextFactory.CreateDbContextAsync(cancellationToken))
        {
            using (var transaction = await context.Database.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    var tenantSettings = await this.tenantSettingsCacheService.GetGlobalTenantSettingsAsync(cancellationToken);
                    if (tenantSettings.IsCustomPeriodOn)
                    {
                        var customPeriod = await TryGetActiveCustomPeriodAsync(context, userId, cancellationToken);
                        if (customPeriod == null)
                            return false;

                        var customPeriodUserChallenges = customPeriod.CustomPeriodUserChallenges.Where(x => x.IsCompleted && !x.IsRewardCollected);

                        if (!customPeriodUserChallenges.Any()) return false;

                        foreach (var challenge in customPeriodUserChallenges)
                        {
                            customPeriod.Points += challenge.RewardPoints;
                            challenge.IsRewardCollected = true;
                        }
                    }
                    else
                    {
                        var userChallenges = await context.UserChallenges
                            .AsTracking()
                            .Include(x => x.Challenge)
                            .Where(x =>
                                x.UserId == userId &&
                                !x.IsActive &&
                                !x.IsRewardCollected &&
                                x.CompletedDate != null &&
                                x.Status == ChallengeStatus.Completed)
                            .ToListAsync(cancellationToken);

                        if (!userChallenges.Any()) return false;

                        var periodPerformance = await TryGetActivePeriodAsync(context, userId, includeRewards: false, cancellationToken);

                        if (periodPerformance == null) return false;

                        foreach (var challenge in userChallenges)
                        {
                            periodPerformance.Points += (int)challenge.Challenge.RewardPoints;
                            challenge.IsRewardCollected = true;
                        }
                    }

                    await SaveAndCommitTransaction(context, transaction, cancellationToken);

                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    logger.LogError(ex,
                        "Error during GameSessionService.CollectRewardsFromChallenges for UserId {UserId}. Exception: {Message}",
                        userId,
                        ex.Message);
                    return false;
                }
            }
        }
    }

    /// <inheritdoc/>
    public async Task EvaluateUserRewards(string userId, CancellationToken cancellationToken)
    {
        using (var context = await this.dbContextFactory.CreateDbContextAsync(cancellationToken))
        {
            using (var transaction = await context.Database.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    var tenantSettings = await this.tenantSettingsCacheService.GetGlobalTenantSettingsAsync(cancellationToken);

                    if (tenantSettings.IsCustomPeriodOn)
                    {
                        var customPeriod = await TryGetActiveCustomPeriodAsync(context, userId, cancellationToken);

                        if (customPeriod == null) return;

                        var customPeriodUserRewards = customPeriod.CustomPeriodUserRewards.Where(x => !x.IsCompleted);

                        if (!customPeriodUserRewards.Any()) return;

                        var completedRewards = new List<UserChallengeReward>();

                        foreach (var userReward in customPeriodUserRewards)
                        {
                            if (customPeriod.Points >= userReward.RequiredPoints)
                            {
                                userReward.IsCompleted = true;
                                completedRewards.Add(new UserChallengeReward
                                {
                                    UserId = userId,
                                    AvailableFromDate = DateTime.UtcNow,
                                    ExpiresDate = DateTime.UtcNow.AddDays(Enum.Parse<TimePeriodType>(tenantSettings.PeriodOfRewardReset).GetDays()),
                                    IsClaimed = false,
                                    RewardId = userReward.RewardId,
                                    IsExpired = false,
                                });
                            }
                        }
                        if (completedRewards.Any())
                            await context.UserChallengeRewards.AddRangeAsync(completedRewards, cancellationToken);
                    }
                    else
                    {
                        var periodPerformance = await TryGetActivePeriodAsync(context, userId, includeRewards: true, cancellationToken);

                        if (periodPerformance == null) return;

                        var userPeriodRewards = periodPerformance.UserChallengePeriodRewards
                            .Where(x => !x.IsCompleted);

                        if (!userPeriodRewards.Any()) return;

                        var completedRewards = new List<UserChallengeReward>();

                        foreach (var item in userPeriodRewards)
                        {
                            if (periodPerformance.Points >= (int)item.ChallengeReward.RequiredPoints)
                            {
                                item.IsCompleted = true;
                                completedRewards.Add(new UserChallengeReward
                                {
                                    UserId = userId,
                                    AvailableFromDate = DateTime.UtcNow,
                                    ExpiresDate = DateTime.UtcNow.AddDays(Enum.Parse<TimePeriodType>(tenantSettings.PeriodOfRewardReset).GetDays()),
                                    IsClaimed = false,
                                    RewardId = item.ChallengeRewardId,
                                    IsExpired = false,
                                });
                            }
                        }

                        if (completedRewards.Any())
                            await context.UserChallengeRewards.AddRangeAsync(completedRewards, cancellationToken);
                    }

                    await SaveAndCommitTransaction(context, transaction, cancellationToken);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    logger.LogError(ex,
                        "Error during GameSessionService.EvaluateUserRewards for UserId {UserId}. Exception: {Message}",
                        userId,
                        ex.Message);
                    return;
                }
            }
        }
    }

    private async Task SaveAndCommitTransaction(TenantDbContext context, IDbContextTransaction transaction, CancellationToken cancellationToken)
    {
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    private async Task<UserChallengePeriodPerformance?> TryGetActivePeriodAsync(TenantDbContext context, string userId, bool includeRewards, CancellationToken cancellationToken)
    {
        var query = context.UserChallengePeriodPerformances
            .AsTracking()
            .Where(x => x.UserId == userId && x.IsPeriodActive);

        if (includeRewards)
        {
            query = query
                .Include(x => x.UserChallengePeriodRewards)
                    .ThenInclude(r => r.ChallengeReward);
        }

        var periods = await query.ToListAsync(cancellationToken);


        if (periods.Count > 1)
        {
            this.logger.LogWarning("Active user period performance can't be more then 1(One). UserId {UserId}", userId);
            return null;
        }
        else if (periods.Count == 0)
        {
            this.logger.LogWarning("There is no active user period performance. UserId {UserId}", userId);
            return null;
        }

        return periods.First();
    }

    private async Task<UserChallengePeriodPerformance?> TryGetActiveCustomPeriodAsync(TenantDbContext context, string userId, CancellationToken cancellationToken)
    {
        var customPeriods = await context.UserChallengePeriodPerformances
           .AsTracking()
           .Include(x => x.CustomPeriodUserChallenges)
           .ThenInclude(x => x.Game)
           .Include(x => x.CustomPeriodUserRewards)
           .ThenInclude(x => x.Reward)
           .Where(x => x.UserId == userId && x.IsPeriodActive)
           .ToListAsync(cancellationToken);

        if (customPeriods.Count == 0)
        {
            this.logger.LogWarning("Active Custom Period was not found for UserId {UserId}", userId);
            return null;
        }
        else if (customPeriods.Count > 1)
        {
            this.logger.LogWarning("Active Custom Period can't be more then 1(One). UserId {UserId}", userId);
            return null;
        }

        return customPeriods.First();
    }
}
