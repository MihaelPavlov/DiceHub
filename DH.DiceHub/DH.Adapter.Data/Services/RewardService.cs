using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.ChallengeHub;
using DH.Domain.Adapters.Statistics;
using DH.Domain.Adapters.Statistics.Services;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Extensions;
using DH.Domain.Helpers;
using DH.Domain.Services;
using DH.Domain.Services.TenantSettingsService;
using DH.OperationResultCore.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DH.Adapter.Data.Services;

public class RewardService : IRewardService
{
    readonly IDbContextFactory<TenantDbContext> _contextFactory;
    readonly IUserContext userContext;
    readonly ITenantSettingsCacheService tenantSettingsCacheService;
    readonly ILogger<RewardService> logger;
    readonly IChallengeHubClient challengeHubClient;
    readonly IStatisticQueuePublisher statisticQueuePublisher;
    public RewardService(
        IDbContextFactory<TenantDbContext> _contextFactory, IUserContext userContext,
        ITenantSettingsCacheService tenantSettingsCacheService, ILogger<RewardService> logger,
        IChallengeHubClient challengeHubClient, IStatisticQueuePublisher statisticQueuePublisher)
    {
        this._contextFactory = _contextFactory;
        this.userContext = userContext;
        this.tenantSettingsCacheService = tenantSettingsCacheService;
        this.logger = logger;
        this.challengeHubClient = challengeHubClient;
        this.statisticQueuePublisher = statisticQueuePublisher;
    }

    public async Task RewardConfirmation(int userChallengeRewardId, CancellationToken cancellationToken)
    {
        var tenantSettings = await this.tenantSettingsCacheService.GetGlobalTenantSettingsAsync(cancellationToken);

        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            using (var transaction = await context.Database.BeginTransactionAsync(cancellationToken))
            {
                var scannedUserReward = await context.UserChallengeRewards.FirstOrDefaultAsync(x => x.Id == userChallengeRewardId, cancellationToken)
                    ?? throw new NotFoundException(nameof(UserChallengeReward), userChallengeRewardId);

                scannedUserReward.IsClaimed = true;

                if (tenantSettings.IsCustomPeriodOn)
                {
                    await ProcessUseXRewardChallengeForUserCustomPeriod(tenantSettings, context, scannedUserReward, cancellationToken);
                }
                else
                {
                    await ProcessUseXRewardChallengeForUserPeriod(tenantSettings, context, scannedUserReward, cancellationToken);
                }

                await context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                await this.statisticQueuePublisher.PublishAsync(new StatisticJobQueue.RewardActionDetectedJob(
                    scannedUserReward.UserId, scannedUserReward.RewardId,
                    CollectedDate: DateTime.UtcNow, ExpiredDate: null,
                    IsExpired: false, IsCollected: scannedUserReward.IsClaimed));
            }
        }

    }

    private async Task<bool> ProcessUseXRewardChallengeForUserPeriod(TenantSetting tenantSettings, TenantDbContext context, UserChallengeReward scannedUserReward, CancellationToken cancellationToken)
    {
        var periods = await context.UserChallengePeriodPerformances
            .AsTracking()
            .Include(x => x.UserChallengePeriodRewards)
                .ThenInclude(r => r.ChallengeReward)
            .Where(x => x.UserId == scannedUserReward.UserId && x.IsPeriodActive)
            .ToListAsync(cancellationToken);

        var activePeriod = periods.GetActiveUserPeriod(this.logger, scannedUserReward.UserId);
        if (activePeriod == null)
            return false;

        var userUniversalChallenges = await context.UserChallenges
               .AsTracking()
               .Include(x => x.UniversalChallenge)
               .Where(x =>
                   x.UserId == scannedUserReward.UserId &&
                   !x.IsActive &&
                   !x.IsRewardCollected &&
                   x.CompletedDate != null &&
                   x.UniversalChallenge != null &&
                   x.Status == ChallengeStatus.Completed)
               .ToListAsync(cancellationToken);

        var useXRewardsChallenge = userUniversalChallenges
            .FirstOrDefault(x => x.UniversalChallenge!.Type == UniversalChallengeType.UseRewards);

        if (useXRewardsChallenge != null)
        {
            useXRewardsChallenge.AttemptCount++;

            if (useXRewardsChallenge.UniversalChallenge!.Attempts == useXRewardsChallenge.AttemptCount)
            {
                useXRewardsChallenge.IsActive = true;
                useXRewardsChallenge.CompletedDate = DateTime.UtcNow;
                useXRewardsChallenge.IsRewardCollected = true;

                activePeriod.Points += (int)useXRewardsChallenge.UniversalChallenge.RewardPoints;

                await this.challengeHubClient.SendUniversalChallengeCompleted(
                    scannedUserReward.UserId,
                    useXRewardsChallenge.UniversalChallenge!.Name_EN,
                    useXRewardsChallenge.UniversalChallenge!.Name_BG,
                    (int)useXRewardsChallenge.UniversalChallenge.RewardPoints);

                foreach (var userPeriodReward in activePeriod.UserChallengePeriodRewards.Where(x => !x.IsCompleted))
                {
                    if (activePeriod.Points >= (int)userPeriodReward.ChallengeReward.RequiredPoints)
                    {
                        userPeriodReward.IsCompleted = true;
                        await context.UserChallengeRewards.AddAsync(new UserChallengeReward
                        {
                            UserId = scannedUserReward.UserId,
                            AvailableFromDate = DateTime.UtcNow,
                            ExpiresDate = DateTime.UtcNow.AddDays(Enum.Parse<TimePeriodType>(tenantSettings.PeriodOfRewardReset).GetDays()),
                            IsClaimed = false,
                            RewardId = userPeriodReward.ChallengeRewardId,
                            IsExpired = false,
                        }, cancellationToken);

                        await this.challengeHubClient.SendRewardGranted(
                            scannedUserReward.UserId,
                            userPeriodReward.ChallengeReward.Name_BG,
                            userPeriodReward.ChallengeReward.Name_EN);
                    }
                }
            }
            else
            {
                await this.challengeHubClient.SendUniversalChallengeUpdated(
                    scannedUserReward.UserId,
                    useXRewardsChallenge.UniversalChallenge!.Name_EN,
                    useXRewardsChallenge.UniversalChallenge!.Name_BG);
            }
        }

        return true;
    }

    private async Task ProcessUseXRewardChallengeForUserCustomPeriod(TenantSetting tenantSettings, TenantDbContext context, UserChallengeReward scannedUserReward, CancellationToken cancellationToken)
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
            .Where(x => x.UserId == scannedUserReward.UserId && x.IsPeriodActive)
            .ToListAsync(cancellationToken);

        var customPeriod = customPeriods.GetActiveUserCustomPeriod(this.logger, scannedUserReward.UserId);
        if (customPeriod == null)
            return;

        var useXRewardsChallenge = customPeriod.CustomPeriodUserUniversalChallenges
                .FirstOrDefault(x => x.UniversalChallenge.Type == UniversalChallengeType.UseRewards);

        if (useXRewardsChallenge != null && !useXRewardsChallenge.IsCompleted)
        {
            useXRewardsChallenge.UserAttempts++;

            if (useXRewardsChallenge.UserAttempts == useXRewardsChallenge.ChallengeAttempts)
            {
                useXRewardsChallenge.IsCompleted = true;
                useXRewardsChallenge.CompletedDate = DateTime.UtcNow;
                useXRewardsChallenge.IsRewardCollected = true;

                customPeriod.Points += useXRewardsChallenge.RewardPoints;

                await this.challengeHubClient.SendUniversalChallengeCompleted(
                    scannedUserReward.UserId,
                    useXRewardsChallenge.UniversalChallenge!.Name_EN,
                    useXRewardsChallenge.UniversalChallenge!.Name_BG,
                    useXRewardsChallenge.RewardPoints);

                foreach (var userCustomPeriodReward in customPeriod.CustomPeriodUserRewards.Where(x => !x.IsCompleted))
                {
                    if (customPeriod.Points >= userCustomPeriodReward.RequiredPoints)
                    {
                        userCustomPeriodReward.IsCompleted = true;
                        await context.UserChallengeRewards.AddAsync(new UserChallengeReward
                        {
                            UserId = scannedUserReward.UserId,
                            AvailableFromDate = DateTime.UtcNow,
                            ExpiresDate = DateTime.UtcNow.AddDays(Enum.Parse<TimePeriodType>(tenantSettings.PeriodOfRewardReset).GetDays()),
                            IsClaimed = false,
                            RewardId = userCustomPeriodReward.RewardId,
                            IsExpired = false,
                        }, cancellationToken);

                        await this.challengeHubClient.SendRewardGranted(
                            scannedUserReward.UserId,
                            userCustomPeriodReward.Reward.Name_BG,
                            userCustomPeriodReward.Reward.Name_EN);
                    }
                }
            }
            else
            {
                await this.challengeHubClient.SendUniversalChallengeUpdated(
                    scannedUserReward.UserId,
                    useXRewardsChallenge.UniversalChallenge!.Name_EN,
                    useXRewardsChallenge.UniversalChallenge!.Name_BG);
            }
        }
    }

    public async Task<int> CreateReward(ChallengeReward reward, string fileName, string contentType, MemoryStream imageStream, CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            using (var transaction = await context.Database.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    reward.CreatedBy = this.userContext.UserId;
                    await context.ChallengeRewards.AddAsync(reward, cancellationToken);

                    await context.ChallengeRewardImages
                        .AddAsync(new ChallengeRewardImage
                        {
                            Reward = reward,
                            FileName = fileName,
                            ContentType = contentType,
                            Data = imageStream.ToArray(),
                        }, cancellationToken);

                    await context.SaveChangesAsync(cancellationToken);

                    await transaction.CommitAsync(cancellationToken);

                    return reward.Id;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            }
        }
    }

    public async Task UpdateReward(ChallengeReward reward, string fileName, string contentType, MemoryStream imageStream, CancellationToken cancellationToken)
    {
        using (var context = await this._contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var dbReward = await context.ChallengeRewards
                .AsTracking()
                .Include(g => g.Image)
                .FirstOrDefaultAsync(x => x.Id == reward.Id, cancellationToken)
                    ?? throw new NotFoundException(nameof(ChallengeReward), reward.Id);

            var oldImage = dbReward.Image;

            dbReward.Name_EN = reward.Name_EN;
            dbReward.Name_BG = reward.Name_BG;
            dbReward.Description_EN = reward.Description_EN;
            dbReward.Description_BG = reward.Description_BG;
            dbReward.CashEquivalent = reward.CashEquivalent;
            dbReward.RequiredPoints = reward.RequiredPoints;
            dbReward.Level = reward.Level;
            dbReward.UpdatedDate = DateTime.UtcNow;
            dbReward.UpdatedBy = this.userContext.UserId;

            var newRewardImage = new ChallengeRewardImage
            {
                FileName = fileName,
                ContentType = contentType,
                Data = imageStream.ToArray(),
            };

            dbReward.Image = newRewardImage;

            context.ChallengeRewardImages.Remove(oldImage);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}