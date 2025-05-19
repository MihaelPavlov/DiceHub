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
using DH.OperationResultCore.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DH.Adapter.Data.Services;

/// <inheritdoc/>
public class GameSessionService : IGameSessionService
{
    readonly IDbContextFactory<TenantDbContext> dbContextFactory;
    readonly SynchronizeUsersChallengesQueue queue;
    readonly ITenantSettingsCacheService tenantSettingsCacheService;
    readonly IStatisticQueuePublisher statisticQueuePublisher;
    readonly IUserService userService;

    public GameSessionService(
        IDbContextFactory<TenantDbContext> dbContextFactory,
        SynchronizeUsersChallengesQueue queue,
        ITenantSettingsCacheService tenantSettingsCacheService,
        IStatisticQueuePublisher statisticQueuePublisher,
        IUserService userService)
    {
        this.dbContextFactory = dbContextFactory;
        this.queue = queue;
        this.tenantSettingsCacheService = tenantSettingsCacheService;
        this.statisticQueuePublisher = statisticQueuePublisher;
        this.userService = userService;
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

                    // Initiation of the next challenge
                    if (completedChallenges.Any())
                    {
                        var lockedChallenges = await context.UserChallenges
                            .Where(uc => uc.UserId == userId && uc.Status == ChallengeStatus.Locked)
                            .ToListAsync(cancellationToken);

                        if (lockedChallenges.Count != 0)
                            this.queue.AddChallengeInitiationJob(userId, DateTime.UtcNow);
                        else
                        {
                            var tenantSettings = await this.tenantSettingsCacheService.GetGlobalTenantSettingsAsync(cancellationToken);
                            this.queue.AddChallengeInitiationJob(userId, DateTime.UtcNow.AddHours(tenantSettings.ChallengeInitiationDelayHours));
                        }
                    }

                    await context.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);

                    return true;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
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

                    if (!userChallenges.Any())
                        return false;

                    var userActivePeriodPerformanceList = await context.UserChallengePeriodPerformances
                        .AsTracking()
                        .Where(x => x.UserId == userId && x.IsPeriodActive)
                        .ToListAsync(cancellationToken);

                    if (userActivePeriodPerformanceList.Count > 1)
                        throw new InfrastructureException($"Active user period performance can't be more then 1(One). user-id {userId}");
                    else if (userActivePeriodPerformanceList.Count == 0)
                        throw new InfrastructureException($"There is no active user period performance. user-id {userId}");

                    var periodPerformance = userActivePeriodPerformanceList.First();

                    foreach (var challenge in userChallenges)
                    {
                        periodPerformance.Points += (int)challenge.Challenge.RewardPoints;
                        challenge.IsRewardCollected = true;
                    }

                    await context.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                    return true;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
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
                    var userActivePeriodPerformanceList = await context.UserChallengePeriodPerformances
                       .Where(x => x.UserId == userId && x.IsPeriodActive)
                       .ToListAsync(cancellationToken);

                    if (userActivePeriodPerformanceList.Count > 1)
                        throw new InfrastructureException($"Active user period performance can't be more then 1(One). user-id {userId}");
                    else if (userActivePeriodPerformanceList.Count == 0)
                        throw new InfrastructureException($"There is no active user period performance. user-id {userId}");

                    var periodPerformance = userActivePeriodPerformanceList.First();

                    var userPeriodRewards = await context.UserChallengePeriodRewards
                        .AsTracking()
                        .Include(x => x.ChallengeReward)
                        .Where(x => x.UserChallengePeriodPerformanceId == periodPerformance.Id && !x.IsCompleted)
                        .ToListAsync(cancellationToken);

                    var completedRewards = new List<UserChallengeReward>();

                    foreach (var item in userPeriodRewards)
                    {
                        if (periodPerformance.Points >= (int)item.ChallengeReward.RequiredPoints)
                        {
                            var tenantSettings = await this.tenantSettingsCacheService.GetGlobalTenantSettingsAsync(cancellationToken);

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
                    await context.UserChallengeRewards.AddRangeAsync(completedRewards, cancellationToken);
                    await context.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            }
        }
    }
}
