using DH.Domain.Adapters.Authentication.Models.Enums;
using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.ChallengeHub;
using DH.Domain.Adapters.ChallengesOrchestrator;
using DH.Domain.Adapters.Statistics;
using DH.Domain.Adapters.Statistics.Services;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Extensions;
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
    readonly ISynchronizeUsersChallengesQueue queue;
    readonly ITenantSettingsCacheService tenantSettingsCacheService;
    readonly IStatisticQueuePublisher statisticQueuePublisher;
    readonly IUserService userService;
    readonly IChallengeHubClient challengeHubClient;
    readonly ILogger<GameSessionService> logger;

    public GameSessionService(
        IDbContextFactory<TenantDbContext> dbContextFactory,
        ISynchronizeUsersChallengesQueue queue,
        ITenantSettingsCacheService tenantSettingsCacheService,
        IStatisticQueuePublisher statisticQueuePublisher,
        IUserService userService,
        IChallengeHubClient challengeHubClient,
        ILogger<GameSessionService> logger)
    {
        this.dbContextFactory = dbContextFactory;
        this.queue = queue;
        this.tenantSettingsCacheService = tenantSettingsCacheService;
        this.statisticQueuePublisher = statisticQueuePublisher;
        this.userService = userService;
        this.challengeHubClient = challengeHubClient;
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

                        var updatedChallenges = await ProcessCustomChallengesAsync(context, customPeriod, userId, gameId, cancellationToken);
                        var updatedUniversalChallenges = await ProcessCustomUniversalChallengesAsync(context, customPeriod, userId, gameId, cancellationToken);

                        foreach (var challenge in updatedUniversalChallenges)
                            await this.challengeHubClient.SendUniversalChallengeUpdated(
                                userId, challenge.UniversalChallenge.Name_EN, challenge.UniversalChallenge.Name_BG);

                        foreach (var challenge in updatedChallenges)
                            await this.challengeHubClient.SendChallengeUpdated(
                                userId, challenge.Game.Name);
                    }
                    else
                    {
                        var userChallenges = await context.UserChallenges
                            .AsTracking()
                            .Include(x => x.Challenge)
                            .ThenInclude(x => x.Game)
                            .Where(x =>
                                x.UserId == userId &&
                                x.IsActive &&
                                x.Status == ChallengeStatus.InProgress &&
                                x.Challenge != null &&
                                gameId == x.Challenge.GameId)
                            .ToListAsync(cancellationToken);

                        var userUniversalChallenges = await context.UserChallenges
                            .AsTracking()
                            .Include(x => x.UniversalChallenge)
                            .Where(x =>
                                x.UserId == userId &&
                                x.IsActive &&
                                x.Status == ChallengeStatus.InProgress &&
                                x.UniversalChallenge != null)
                            .ToListAsync(cancellationToken);

                        if (!userChallenges.Any() && !userUniversalChallenges.Any())
                            return false;

                        var (updatedChallenges, completedChallenges) = await ProcessUserChallenges(userId, context, userChallenges, cancellationToken);
                        var (updatedUniversalChallenges, completedUniversalChallenges) = await ProcessUserUniversalChallenges(userId, gameId, context, userUniversalChallenges, cancellationToken);

                        foreach (var challenge in updatedChallenges)
                        {
                            await this.challengeHubClient.SendChallengeUpdated(
                                userId, challenge.Challenge!.Game.Name);
                        }

                        foreach (var challenge in updatedUniversalChallenges)
                        {
                            await this.challengeHubClient.SendUniversalChallengeUpdated(
                                userId, challenge.UniversalChallenge!.Name_EN, challenge.UniversalChallenge!.Name_BG);
                        }

                        if (!completedUniversalChallenges.Any() && !completedChallenges.Any()) return false;

                        if (completedChallenges.Any())
                        {
                            var lockedChallenges = await context.UserChallenges
                                .Where(uc => uc.UserId == userId && uc.Status == ChallengeStatus.Locked)
                                .ToListAsync(cancellationToken);

                            if (lockedChallenges.Count != 0)
                                await this.queue.AddChallengeInitiationJob(userId, DateTime.UtcNow);
                            else
                            {
                                await this.queue.AddChallengeInitiationJob(userId, DateTime.UtcNow.AddHours(tenantSettings.ChallengeInitiationDelayHours));
                            }
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

    #region Helpers ProcessChallengeAfterSession

    private async Task<(List<UserChallenge> updatedUniversalChallenges, List<UserChallenge> completedUniversalChallenges)> ProcessUserUniversalChallenges(
        string userId, int gameId, TenantDbContext context, List<UserChallenge> userChallenges, CancellationToken cancellationToken)
    {
        var validTypes = new[]
        {
            UniversalChallengeType.PlayGames,
            UniversalChallengeType.PlayFavoriteGame,
            UniversalChallengeType.JoinMeepleRooms
        };

        var updatedChallenges = new List<UserChallenge>();
        userChallenges = userChallenges.Where(x => validTypes.Contains(x.UniversalChallenge!.Type)).ToList();
        foreach (var challenge in userChallenges)
        {
            switch (challenge.UniversalChallenge!.Type)
            {
                case UniversalChallengeType.PlayGames:
                    challenge.AttemptCount++;
                    updatedChallenges.Add(challenge);
                    break;

                case UniversalChallengeType.PlayFavoriteGame when challenge.GameId == gameId:
                    challenge.AttemptCount++;
                    updatedChallenges.Add(challenge);
                    break;

                case UniversalChallengeType.JoinMeepleRooms:
                    if (await HasUserJoinedMeepleRoomToday(context, userId, gameId, cancellationToken))
                    {
                        challenge.AttemptCount++;
                        updatedChallenges.Add(challenge);
                    }
                    break;
            }
        }

        var completedChallenges = userChallenges.Where(x => x.AttemptCount == x.UniversalChallenge!.Attempts).ToList();

        if (!completedChallenges.Any())
            return (updatedChallenges, []);

        foreach (var challenge in completedChallenges)
        {
            challenge.CompletedDate = DateTime.UtcNow;
            challenge.Status = ChallengeStatus.Completed;
            challenge.IsActive = false;
            var challengeForRemove = updatedChallenges.FirstOrDefault(x => x.Id == challenge.Id);

            if (challengeForRemove != null)
                updatedChallenges.Remove(challengeForRemove);

            await this.challengeHubClient.SendUniversalChallengeCompleted(
                userId, challenge.UniversalChallenge!.Name_EN,
                challenge.UniversalChallenge.Name_BG,
                (int)challenge.UniversalChallenge.RewardPoints);

            if (!await this.userService.HasUserAnyMatchingRole(userId, Role.SuperAdmin))
            {
                await this.statisticQueuePublisher.PublishAsync(new ChallengeProcessingOutcomeJob(
                    userId,
                    100000 + challenge.UniversalChallengeId!.Value,
                    ChallengeOutcome.Completed,
                    challenge.CompletedDate.Value,
                    DateTime.UtcNow));
            }
        }

        return (updatedChallenges, completedChallenges);
    }

    private async Task<(List<UserChallenge> updatedChallenges, List<UserChallenge> completedChallenge)> ProcessUserChallenges(
        string userId, TenantDbContext context, List<UserChallenge> userChallenges, CancellationToken cancellationToken)
    {
        var updatedChallenges = new List<UserChallenge>();
        foreach (var userChallenge in userChallenges)
        {
            var currentAttempts = userChallenge.AttemptCount;
            var challengeAttempts = userChallenge.Challenge!.Attempts;

            if (currentAttempts < challengeAttempts)
            {
                userChallenge.AttemptCount++;
                updatedChallenges.Add(userChallenge);
            }
        }

        var completedChallenges = userChallenges.Where(x => x.AttemptCount == x.Challenge!.Attempts).ToList();

        if (!completedChallenges.Any())
        {
            return (updatedChallenges, []);
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
            var challengeForRemove = updatedChallenges.FirstOrDefault(x => x.Id == challenge.Id);

            if (challengeForRemove != null)
                updatedChallenges.Remove(challengeForRemove);

            await this.challengeHubClient.SendChallengeCompleted(
                userId, challenge.Challenge!.Game.Name, (int)challenge.Challenge.RewardPoints);

            if (!await this.userService.HasUserAnyMatchingRole(userId, Role.SuperAdmin))
            {
                await this.statisticQueuePublisher.PublishAsync(new ChallengeProcessingOutcomeJob(
                    userId,
                    challenge.ChallengeId!.Value,
                    ChallengeOutcome.Completed,
                    challenge.CompletedDate.Value,
                    DateTime.UtcNow));
            }

            var challengeStats = challengeStatistics.First(x => x.ChallengeId == challenge.ChallengeId);
            challengeStats.TotalCompletions++;
        }

        return (updatedChallenges, completedChallenges);
    }

    #endregion Helpers ProcessChallengeAfterSession

    #region Helpers ProcessChallengeAfterSession CustomPeriod
    private async Task<List<CustomPeriodUserChallenge>> ProcessCustomChallengesAsync(
        TenantDbContext context,
        UserChallengePeriodPerformance customPeriod,
        string userId, int gameId,
        CancellationToken cancellationToken)
    {
        var challenges = customPeriod.CustomPeriodUserChallenges
            .Where(x => !x.IsCompleted && x.GameId == gameId)
            .ToList();

        var updated = new List<CustomPeriodUserChallenge>();

        foreach (var challenge in challenges)
        {
            if (challenge.UserAttempts < challenge.ChallengeAttempts)
            {
                challenge.UserAttempts++;
                updated.Add(challenge);
            }
        }

        var completed = challenges.Where(x => x.UserAttempts == x.ChallengeAttempts).ToList();
        foreach (var challenge in completed)
        {
            challenge.CompletedDate = DateTime.UtcNow;
            challenge.IsCompleted = true;

            updated.RemoveAll(x => x.Id == challenge.Id);

            await this.challengeHubClient.SendChallengeCompleted(userId, challenge.Game.Name, challenge.RewardPoints);

            if (!await userService.HasUserAnyMatchingRole(userId, Role.SuperAdmin))
            {
                await statisticQueuePublisher.PublishAsync(new ChallengeProcessingOutcomeJob(
                    userId,
                    10000 + challenge.Id,
                    ChallengeOutcome.Completed,
                    challenge.CompletedDate!.Value,
                    DateTime.UtcNow));
            }
        }

        return updated;
    }

    private async Task<List<CustomPeriodUserUniversalChallenge>> ProcessCustomUniversalChallengesAsync(
        TenantDbContext context,
        UserChallengePeriodPerformance customPeriod,
        string userId, int gameId,
        CancellationToken cancellationToken)
    {
        var validTypes = new[]
        {
            UniversalChallengeType.PlayGames,
            UniversalChallengeType.PlayFavoriteGame,
            UniversalChallengeType.JoinMeepleRooms
        };

        var challenges = customPeriod.CustomPeriodUserUniversalChallenges
            .Where(x => !x.IsCompleted && validTypes.Contains(x.UniversalChallenge.Type))
            .ToList();

        var updated = new List<CustomPeriodUserUniversalChallenge>();

        foreach (var challenge in challenges)
        {
            switch (challenge.UniversalChallenge.Type)
            {
                case UniversalChallengeType.PlayGames:
                    challenge.UserAttempts++;
                    updated.Add(challenge);
                    break;

                case UniversalChallengeType.PlayFavoriteGame when challenge.GameId == gameId:
                    challenge.UserAttempts++;
                    updated.Add(challenge);
                    break;

                case UniversalChallengeType.JoinMeepleRooms:
                    if (await HasUserJoinedMeepleRoomToday(context, userId, gameId, cancellationToken))
                    {
                        challenge.UserAttempts++;
                        updated.Add(challenge);
                    }
                    break;
            }
        }

        var completed = challenges.Where(x => x.UserAttempts == x.ChallengeAttempts).ToList();

        foreach (var challenge in completed)
        {
            challenge.CompletedDate = DateTime.UtcNow;
            challenge.IsCompleted = true;

            updated.RemoveAll(x => x.Id == challenge.Id);

            await this.challengeHubClient.SendUniversalChallengeCompleted(
                userId, challenge.UniversalChallenge.Name_EN, challenge.UniversalChallenge.Name_BG, challenge.RewardPoints);

            if (!await userService.HasUserAnyMatchingRole(userId, Role.SuperAdmin))
            {
                await statisticQueuePublisher.PublishAsync(new ChallengeProcessingOutcomeJob(
                    userId,
                    10000 + challenge.Id,
                    ChallengeOutcome.Completed,
                    challenge.CompletedDate!.Value,
                    DateTime.UtcNow));
            }
        }

        return updated;
    }

    private async Task<bool> HasUserJoinedMeepleRoomToday(
        TenantDbContext context, string userId, int gameId, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;

        var createdRoom = await context.Rooms
            .FirstOrDefaultAsync(x => x.UserId == userId && x.GameId == gameId && x.StartDate.Date == today, cancellationToken);

        if (createdRoom != null)
            return true;

        var participantRoom = await context.RoomParticipants
            .Include(x => x.Room)
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Room.GameId == gameId && x.Room.StartDate.Date == today, cancellationToken);

        return participantRoom != null;
    }

    #endregion Helpers ProcessChallengeAfterSession CustomPeriod

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
                        var customPeriodUserUniversalChallenges = customPeriod.CustomPeriodUserUniversalChallenges.Where(x => x.IsCompleted && !x.IsRewardCollected);

                        foreach (var challenge in customPeriodUserChallenges)
                        {
                            customPeriod.Points += challenge.RewardPoints;
                            challenge.IsRewardCollected = true;
                        }

                        foreach (var challenge in customPeriodUserUniversalChallenges)
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
                                x.Challenge != null &&
                                x.Status == ChallengeStatus.Completed)
                            .ToListAsync(cancellationToken);

                        var userUniversalChallenges = await context.UserChallenges
                            .AsTracking()
                            .Include(x => x.UniversalChallenge)
                            .Where(x =>
                                x.UserId == userId &&
                                !x.IsActive &&
                                !x.IsRewardCollected &&
                                x.CompletedDate != null &&
                                x.UniversalChallenge != null &&
                                x.Status == ChallengeStatus.Completed)
                            .ToListAsync(cancellationToken);

                        if (!userChallenges.Any() && !userUniversalChallenges.Any()) return false;

                        var periodPerformance = await TryGetActivePeriodAsync(context, userId, includeRewards: false, cancellationToken);

                        if (periodPerformance == null) return false;

                        foreach (var challenge in userChallenges)
                        {
                            periodPerformance.Points += (int)challenge.Challenge!.RewardPoints;
                            challenge.IsRewardCollected = true;
                        }
                        foreach (var challenge in userUniversalChallenges)
                        {
                            periodPerformance.Points += (int)challenge.UniversalChallenge!.RewardPoints;
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

                        ProcessEligibleUserRewardsForCustomPeriod(userId, tenantSettings, customPeriod, customPeriodUserRewards, completedRewards);

                        var rewardGrantedChallenge = customPeriod.CustomPeriodUserUniversalChallenges
                            .FirstOrDefault(x => x.UniversalChallenge.Type == UniversalChallengeType.RewardsGranted);

                        if (rewardGrantedChallenge != null && completedRewards.Any())
                        {
                            var isChallengeCompleted = await HandleCustomRewardsGrantedChallengeAsync(userId, customPeriod, completedRewards, rewardGrantedChallenge);

                            if (isChallengeCompleted) // If this universal challenge just completed, recheck rewards again(as total points changed)
                                ProcessEligibleUserRewardsForCustomPeriod(userId, tenantSettings, customPeriod, customPeriodUserRewards, completedRewards);
                        }

                        if (completedRewards.Any())
                            await context.UserChallengeRewards.AddRangeAsync(completedRewards, cancellationToken);

                        foreach (var reward in completedRewards)
                        {
                            await this.challengeHubClient.SendRewardGranted(
                                userId, reward.Reward.Name_BG, reward.Reward.Name_EN);
                        }
                    }
                    else
                    {
                        var periodPerformance = await TryGetActivePeriodAsync(context, userId, includeRewards: true, cancellationToken);

                        if (periodPerformance == null) return;

                        var userPeriodRewards = periodPerformance.UserChallengePeriodRewards
                            .Where(x => !x.IsCompleted);

                        if (!userPeriodRewards.Any()) return;

                        var completedRewards = new List<UserChallengeReward>();

                        ProcessEligibleUserRewardsForPeriod(userId, tenantSettings, periodPerformance, userPeriodRewards, completedRewards);

                        var userUniversalChallenges = await context.UserChallenges
                            .AsTracking()
                            .Include(x => x.UniversalChallenge)
                            .Where(x =>
                                x.UserId == userId &&
                                x.IsActive &&
                                x.Status == ChallengeStatus.InProgress &&
                                x.UniversalChallenge != null)
                            .ToListAsync(cancellationToken);

                        var rewardGrantedChallenge = userUniversalChallenges
                            .FirstOrDefault(x => x.UniversalChallenge?.Type == UniversalChallengeType.RewardsGranted);

                        if (rewardGrantedChallenge != null && completedRewards.Any())
                        {
                            bool isChallengeCompleted = await HandleRewardsGrantedChallengeAsync(userId, periodPerformance, completedRewards, rewardGrantedChallenge);

                            if (isChallengeCompleted) // If this universal challenge just completed, recheck rewards again(as total points changed)
                                ProcessEligibleUserRewardsForPeriod(userId, tenantSettings, periodPerformance, userPeriodRewards, completedRewards);
                        }

                        if (completedRewards.Any())
                            await context.UserChallengeRewards.AddRangeAsync(completedRewards, cancellationToken);

                        foreach (var reward in completedRewards)
                        {
                            await this.challengeHubClient.SendRewardGranted(
                                userId, reward.Reward.Name_BG, reward.Reward.Name_EN);
                        }
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

    #region EvaluateUserRewards Helper Methods

    private async Task<bool> HandleRewardsGrantedChallengeAsync(
        string userId, UserChallengePeriodPerformance periodPerformance, List<UserChallengeReward> completedRewards, UserChallenge rewardGrantedChallenge)
    {
        rewardGrantedChallenge.AttemptCount += completedRewards.Count;

        if (rewardGrantedChallenge.AttemptCount < rewardGrantedChallenge.UniversalChallenge!.Attempts)
        {
            await challengeHubClient.SendUniversalChallengeUpdated(
                userId,
                rewardGrantedChallenge.UniversalChallenge.Name_EN,
                rewardGrantedChallenge.UniversalChallenge.Name_BG);

            return false;
        }

        rewardGrantedChallenge.Status = ChallengeStatus.Completed;
        rewardGrantedChallenge.CompletedDate = DateTime.UtcNow;
        rewardGrantedChallenge.IsRewardCollected = true;
        rewardGrantedChallenge.IsActive = false;

        periodPerformance.Points += (int)rewardGrantedChallenge.UniversalChallenge.RewardPoints;

        await this.challengeHubClient.SendUniversalChallengeCompleted(
            userId,
            rewardGrantedChallenge.UniversalChallenge.Name_EN,
            rewardGrantedChallenge.UniversalChallenge.Name_BG,
            (int)rewardGrantedChallenge.UniversalChallenge.RewardPoints);

        if (!await userService.HasUserAnyMatchingRole(userId, Role.SuperAdmin))
        {
            await statisticQueuePublisher.PublishAsync(new ChallengeProcessingOutcomeJob(
                userId,
                10000 + rewardGrantedChallenge.Id,
                ChallengeOutcome.Completed,
                rewardGrantedChallenge.CompletedDate!.Value,
                DateTime.UtcNow));
        }

        return true;
    }

    private static void ProcessEligibleUserRewardsForPeriod(
        string userId, TenantSetting tenantSettings, UserChallengePeriodPerformance periodPerformance,
        IEnumerable<UserChallengePeriodReward> userPeriodRewards, List<UserChallengeReward> completedRewards)
    {
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
    }

    private async Task<bool> HandleCustomRewardsGrantedChallengeAsync(
        string userId, UserChallengePeriodPerformance customPeriod,
        List<UserChallengeReward> completedRewards, CustomPeriodUserUniversalChallenge rewardGrantedChallenge)
    {
        rewardGrantedChallenge.UserAttempts += completedRewards.Count;

        if (rewardGrantedChallenge.UserAttempts < rewardGrantedChallenge.ChallengeAttempts)
        {
            await challengeHubClient.SendUniversalChallengeUpdated(
                userId,
                rewardGrantedChallenge.UniversalChallenge.Name_EN,
                rewardGrantedChallenge.UniversalChallenge.Name_BG);

            return false;
        }

        rewardGrantedChallenge.IsCompleted = true;
        rewardGrantedChallenge.CompletedDate = DateTime.UtcNow;
        rewardGrantedChallenge.IsRewardCollected = true;

        customPeriod.Points += rewardGrantedChallenge.RewardPoints;

        await this.challengeHubClient.SendUniversalChallengeCompleted(
            userId,
            rewardGrantedChallenge.UniversalChallenge.Name_EN,
            rewardGrantedChallenge.UniversalChallenge.Name_BG,
            rewardGrantedChallenge.RewardPoints);

        if (!await userService.HasUserAnyMatchingRole(userId, Role.SuperAdmin))
        {
            await statisticQueuePublisher.PublishAsync(new ChallengeProcessingOutcomeJob(
                userId,
                10000 + rewardGrantedChallenge.Id,
                ChallengeOutcome.Completed,
                rewardGrantedChallenge.CompletedDate!.Value,
                DateTime.UtcNow));
        }

        return true;
    }

    private static void ProcessEligibleUserRewardsForCustomPeriod(
        string userId, TenantSetting tenantSettings, UserChallengePeriodPerformance customPeriod,
        IEnumerable<CustomPeriodUserReward> customPeriodUserRewards, List<UserChallengeReward> completedRewards)
    {
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
    }

    #endregion EvaluateUserRewards Helper Methods

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

        return periods.GetActiveUserPeriod(this.logger, userId);
    }

    private async Task<UserChallengePeriodPerformance?> TryGetActiveCustomPeriodAsync(TenantDbContext context, string userId, CancellationToken cancellationToken)
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

        return customPeriods.GetActiveUserCustomPeriod(this.logger, userId);
    }
}
