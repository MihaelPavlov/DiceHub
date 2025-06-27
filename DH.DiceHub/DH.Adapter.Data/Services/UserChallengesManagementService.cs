using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Helpers;
using DH.Domain.Services;
using DH.Domain.Services.TenantSettingsService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DH.Adapter.Data.Services;

/// <inheritdoc/>
public class UserChallengesManagementService : IUserChallengesManagementService
{
    readonly IDbContextFactory<TenantDbContext> dbContextFactory;
    readonly ITenantSettingsCacheService tenantSettingsCacheService;
    readonly ILogger<UserChallengesManagementService> logger;
    readonly IUserService userService;

    public UserChallengesManagementService(
        IDbContextFactory<TenantDbContext> dbContextFactory,
        ITenantSettingsCacheService tenantSettingsCacheService,
        ILogger<UserChallengesManagementService> logger,
        IUserService userService)
    {
        this.dbContextFactory = dbContextFactory;
        this.tenantSettingsCacheService = tenantSettingsCacheService;
        this.logger = logger;
        this.userService = userService;
    }

    /// <inheritdoc/>
    public async Task AddChallengeToUser(string userId, CancellationToken cancellationToken)
    {
        using (var context = await this.dbContextFactory.CreateDbContextAsync(cancellationToken))
        {
            using (var transaction = await context.Database.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    // Fetch active challenges for the user (InProgress)
                    var activeChallenges = await context.UserChallenges
                        .Where(uc => uc.UserId == userId && uc.Status == ChallengeStatus.InProgress)
                        .ToListAsync(cancellationToken);

                    // Fetch locked challenges for the user
                    var lockedChallenges = await context.UserChallenges
                        .AsTracking()
                        .Where(uc => uc.UserId == userId && uc.Status == ChallengeStatus.Locked)
                        .ToListAsync(cancellationToken);

                    // Fetch available system challenges that are not yet assigned to the user
                    var systemChallenges = await context.Challenges
                        .Where(sc => !context.UserChallenges.Any(uc => uc.UserId == userId && uc.ChallengeId == sc.Id))
                        .ToListAsync(cancellationToken);

                    UserChallenge? newUserChallenge = null;

                    if (lockedChallenges.Count != 0)
                    {
                        var lockedChallenge = lockedChallenges.First();
                        lockedChallenge.Status = ChallengeStatus.InProgress;
                        await context.SaveChangesAsync(cancellationToken);
                        await transaction.CommitAsync(cancellationToken);
                        return;
                    }

                    // Handle adding new challenges based on the current active count
                    if (activeChallenges.Count == 0 || activeChallenges.Count == 1)
                    {
                        // User has no active challenges, add one active challenge
                        // User has one active challenge, add another active challenge
                        newUserChallenge = AddNewChallenge(userId, systemChallenges, ChallengeStatus.InProgress, 0);
                    }
                    else if (activeChallenges.Count == 2)
                    {
                        // User has two active challenges, add a locked challenge
                        var minRequiredPoints = activeChallenges.Min(ac => ac.Challenge.RewardPoints);

                        // Generate a locked challenge with required points
                        newUserChallenge = AddNewChallenge(userId, systemChallenges, ChallengeStatus.Locked, minRequiredPoints);
                    }

                    if (newUserChallenge != null)
                    {
                        await context.UserChallenges.AddAsync(newUserChallenge, cancellationToken);
                        await context.SaveChangesAsync(cancellationToken);
                        await transaction.CommitAsync(cancellationToken);
                    }
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            }
        }
    }

    public async Task EnsureValidUserChallengePeriodsAsync(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow.Date;

        using (var context = await this.dbContextFactory.CreateDbContextAsync(cancellationToken))
        {
            var userIds = await this.userService.GetAllUserIds(cancellationToken);
            foreach (var userId in userIds)
            {
                using (var transaction = await context.Database.BeginTransactionAsync(cancellationToken))
                {
                    try
                    {
                        var userPerformance = await context.UserChallengePeriodPerformances.AsTracking()
                            .FirstOrDefaultAsync(x => x.IsPeriodActive == true && x.UserId == userId, cancellationToken);

                        var isInvalid = userPerformance == null || now < userPerformance.StartDate || now > userPerformance.EndDate;

                        if (!isInvalid)
                        {
                            // Skip this user — challenge period is valid
                            continue;
                        }

                        if (userPerformance != null)
                        {
                            userPerformance.IsPeriodActive = false;
                            context.Update(userPerformance);
                        }

                        var tenantSettings = await this.tenantSettingsCacheService.GetGlobalTenantSettingsAsync(cancellationToken);

                        var settingPeriod = Enum.Parse<TimePeriodType>(tenantSettings.PeriodOfRewardReset);
                        var nextResetDate = TimePeriodTypeHelper.CalculateNextResetDate(settingPeriod, tenantSettings.ResetDayForRewards);

                        var newUserPerformance = new UserChallengePeriodPerformance
                        {
                            UserId = userId,
                            IsPeriodActive = true,
                            Points = 0,
                            StartDate = DateTime.UtcNow.Date,
                            EndDate = nextResetDate,
                            TimePeriodType = settingPeriod
                        };

                        await context.UserChallengePeriodPerformances.AddAsync(newUserPerformance, cancellationToken);
                        await context.SaveChangesAsync(cancellationToken);
                        var userChallengePeriodRewards = await this.GenerateRewardsAsyncV3(tenantSettings.ChallengeRewardsCountForPeriod, newUserPerformance.Id, context, cancellationToken);
                        await context.UserChallengePeriodRewards.AddRangeAsync(userChallengePeriodRewards, cancellationToken);
                        // rewards points and not order correctly
                        await context.SaveChangesAsync(cancellationToken);
                        await transaction.CommitAsync(cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        this.logger.LogError("Error appear while new challenge period was adding for user - {userId}. Ex-> {exception}", userId, ex.Message);
                    }
                }
            }
        }
    }

    public async Task<bool> InitiateUserChallengePeriod(string userId, CancellationToken cancellationToken)
    {
        using (var context = await this.dbContextFactory.CreateDbContextAsync(cancellationToken))
        {
            using (var transaction = await context.Database.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    var tenantSettings = await this.tenantSettingsCacheService.GetGlobalTenantSettingsAsync(cancellationToken);

                    var settingPeriod = Enum.Parse<TimePeriodType>(tenantSettings.PeriodOfRewardReset);
                    var nextResetDate = TimePeriodTypeHelper.CalculateNextResetDate(settingPeriod, tenantSettings.ResetDayForRewards);

                    var userPerformance = new UserChallengePeriodPerformance
                    {
                        UserId = userId,
                        IsPeriodActive = true,
                        Points = 0,
                        StartDate = DateTime.UtcNow.Date,
                        EndDate = nextResetDate,
                        TimePeriodType = settingPeriod
                    };

                    await context.UserChallengePeriodPerformances.AddAsync(userPerformance, cancellationToken);
                    await context.SaveChangesAsync(cancellationToken);

                    var userChallengePeriodRewards = await this.GenerateRewardsAsyncV3(tenantSettings.ChallengeRewardsCountForPeriod, userPerformance.Id, context, cancellationToken);
                    // rewards points and not order correctly
                    await context.UserChallengePeriodRewards.AddRangeAsync(userChallengePeriodRewards, cancellationToken);
                    await context.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);

                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    this.logger.LogError("Error appear while new challenge period was adding for user - {userId}. Ex-> {exception}", userId, ex.Message);
                    return false;
                }
            }
        }
    }

    /// <inheritdoc/>
    //IMPORTANT! Challenge period time will be the same for everybody
    //IMPORTANT! Every Sunday at 12:00 PM reset all reward for everybody 
    public async Task InitiateNewUserChallengePeriod(string userId, CancellationToken cancellationToken)
    {
        using (var context = await this.dbContextFactory.CreateDbContextAsync(cancellationToken))
        {
            using (var transaction = await context.Database.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    var tenantSettings = await this.tenantSettingsCacheService.GetGlobalTenantSettingsAsync(cancellationToken);

                    var settingPeriod = Enum.Parse<TimePeriodType>(tenantSettings.PeriodOfRewardReset);
                    var nextResetDate = TimePeriodTypeHelper.CalculateNextResetDate(settingPeriod, tenantSettings.ResetDayForRewards);

                    var userPerformance = new UserChallengePeriodPerformance
                    {
                        UserId = userId,
                        IsPeriodActive = true,
                        Points = 0,
                        StartDate = DateTime.UtcNow.Date,
                        EndDate = nextResetDate,
                        TimePeriodType = settingPeriod
                    };

                    var userChallengePeriodRewards = await this.GenerateRewardsAsyncV3(tenantSettings.ChallengeRewardsCountForPeriod, userPerformance.Id, context, cancellationToken);
                    var userChallenges = await this.GenerateChallengesAsync(userId, context, cancellationToken);

                    userPerformance.UserChallengePeriodRewards = userChallengePeriodRewards;
                    await context.UserChallengePeriodPerformances.AddAsync(userPerformance, cancellationToken);
                    await context.UserChallengePeriodRewards.AddRangeAsync(userChallengePeriodRewards, cancellationToken);
                    await context.UserChallenges.AddRangeAsync(userChallenges, cancellationToken);
                    await context.UserStatistics.AddAsync(new UserStatistic
                    {
                        TotalChallengesCompleted = 0,
                        UserId = userId,
                    });

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

    /// <summary>
    /// Adds a new challenge to a user by selecting a random challenge from the available challenges. 
    /// The status and required points for the challenge are provided.
    /// </summary>
    /// <param name="userId">The ID of the user to whom the challenge is added.</param>
    /// <param name="availableChallenges">A list of available challenges to select from.</param>
    /// <param name="status">The status of the new challenge (e.g., InProgress, Locked).</param>
    /// <param name="rewardPoint">The required points for completing the challenge.</param>
    /// <returns>Returning user challenge</returns>
    private UserChallenge AddNewChallenge(string userId, List<Challenge> availableChallenges, ChallengeStatus status, ChallengeRewardPoint rewardPoint)
    {
        var randomIndex = new Random().Next(availableChallenges.Count);
        var selectedChallenge = availableChallenges[randomIndex];

        return new UserChallenge
        {
            CreatedDate = DateTime.UtcNow,
            UserId = userId,
            ChallengeId = selectedChallenge.Id,
            RequiredUserTotalPoints = rewardPoint,
            AttemptCount = 0,
            Status = status,
            IsActive = true,
        };
    }

    /// <summary>
    /// Generates a list of challenges for a user. 
    /// The first two challenges are marked as InProgress with zero required points, and the third is Locked with points based on the previous challenges.
    /// </summary>
    /// <param name="userId">The ID of the user for whom to generate challenges.</param>
    /// <param name="dbContext">The database context to fetch challenges and manage user data.</param>
    /// <param name="cancellationToken">Token for canceling the task if needed.</param>
    /// <returns>A list of generated user challenges.</returns>
    private async Task<List<UserChallenge>> GenerateChallengesAsync(string userId, TenantDbContext dbContext, CancellationToken cancellationToken)
    {
        // Fetch all system challenges from the database
        var systemChallenges = await dbContext.Challenges.ToListAsync(cancellationToken);

        // List to store the user challenges
        var userChallenges = new List<UserChallenge>();

        // Random generator
        var random = new Random();
        var counterChallenges = 2;
        // 1. Assign the first two challenges with InProgress status and RequiredUserTotalPoints = 0
        while (counterChallenges != 0)
        {
            // Select a random challenge from the system challenges
            var randomIndex = random.Next(systemChallenges.Count);
            var challenge = systemChallenges[randomIndex];

            // Ensure the challenge isn't already added
            if (userChallenges.All(x => x.ChallengeId != challenge.Id))
            {
                userChallenges.Add(new UserChallenge
                {
                    CreatedDate = DateTime.UtcNow,
                    Challenge = challenge,
                    ChallengeId = challenge.Id,
                    RequiredUserTotalPoints = 0, // Set required points to 0 for the first two challenges
                    AttemptCount = 0,
                    Status = ChallengeStatus.InProgress, // InProgress status for the first two challenges
                    UserId = userId,
                    IsActive = true
                });
                counterChallenges--;
            }
        }

        var completedChallenges = userChallenges.Take(2).ToList();
        var minRequiredPoints = completedChallenges.Min(c => c.Challenge.RewardPoints);

        // Assign the third challenge, which is locked, with the minimum RequiredUserTotalPoints
        // Select a random challenge not already assigned
        while (true)
        {
            var thirdRandomIndex = random.Next(systemChallenges.Count);
            var thirdChallenge = systemChallenges[thirdRandomIndex];

            if (userChallenges.All(x => x.ChallengeId != thirdChallenge.Id))
            {
                userChallenges.Add(new UserChallenge
                {
                    CreatedDate = DateTime.UtcNow,
                    RequiredUserTotalPoints = minRequiredPoints,
                    Challenge = thirdChallenge,
                    ChallengeId = thirdChallenge.Id,
                    AttemptCount = 0,
                    Status = ChallengeStatus.Locked,
                    UserId = userId,
                    IsActive = true
                });
                break;
            }
        }

        return userChallenges;
    }

    /// <summary>
    /// Generates rewards for a user challenge period by dividing the total rewards into Bronze, Silver, Gold, and Platinum categories.
    /// Ensures that each reward is unique based on the user's performance.
    /// </summary>
    /// <param name="rewardCount">The total number of rewards to generate.</param>
    /// <param name="userPerformanceId">The ID of the user's performance period.</param>
    /// <param name="dbContext">The database context to interact with rewards and user data.</param>
    /// <param name="cancellationToken">Token for canceling the task if needed.</param>
    /// <returns>A list of user challenge period rewards ordered by required points.</returns>
    private async Task<List<UserChallengePeriodReward>> GenerateRewardsAsyncV3(int rewardCount, int userPerformanceId, TenantDbContext dbContext, CancellationToken cancellationToken)
    {
        // Fetch all rewards from the database
        var systemRewards = await dbContext.ChallengeRewards
            .OrderBy(r => r.RequiredPoints)
            .ToListAsync(cancellationToken);

        // List to store the final rewards for the user
        var userChallengePeriodRewards = new List<UserChallengePeriodReward>();

        // Track the required points already used to ensure uniqueness
        var usedPoints = new HashSet<RewardRequiredPoint>();

        // 1. Divide rewards between Bronze and Silver
        int bronzeCount = rewardCount / 4; // Example: 25% of rewards as Bronze
        int silverCount = rewardCount / 4; // Example: 25% of rewards as Silver

        // Assign Bronze rewards
        AssignRewards(RewardLevel.Bronze, bronzeCount, systemRewards, userChallengePeriodRewards, usedPoints, userPerformanceId);

        // Assign Silver rewards
        AssignRewards(RewardLevel.Silver, silverCount, systemRewards, userChallengePeriodRewards, usedPoints, userPerformanceId);

        // 2. Assign Gold rewards
        int goldCount = rewardCount - bronzeCount - silverCount - 1; // Remaining rewards except for the Platinum
        AssignRewards(RewardLevel.Gold, goldCount, systemRewards, userChallengePeriodRewards, usedPoints, userPerformanceId);

        // 3. Assign the final Platinum reward
        var availablePlatinumRewards = systemRewards.Where(r => r.Level == RewardLevel.Platinum).ToList();
        if (availablePlatinumRewards.Any() && userChallengePeriodRewards.Count < rewardCount)
        {
            var selectedPlatinumReward = SelectUniqueReward(availablePlatinumRewards, usedPoints);

            if (selectedPlatinumReward != null)
            {
                userChallengePeriodRewards.Add(new UserChallengePeriodReward
                {
                    ChallengeRewardId = selectedPlatinumReward.Id,
                    UserChallengePeriodPerformanceId = userPerformanceId,
                    ChallengeReward = selectedPlatinumReward
                });
            }
        }

        // Return the list ordered by required points
        return userChallengePeriodRewards.OrderBy(r => r.ChallengeReward.RequiredPoints).ToList();
    }

    /// <summary>
    /// Assigns a specific number of rewards at a given reward level to a user. 
    /// Ensures that the assigned rewards are unique based on their required points.
    /// </summary>
    /// <param name="rewardLevel">The level of rewards (e.g., Bronze, Silver, Gold, Platinum).</param>
    /// <param name="count">The number of rewards to assign at the specified level.</param>
    /// <param name="allRewards">A list of all available rewards to select from.</param>
    /// <param name="userRewards">The list of user rewards being generated.</param>
    /// <param name="usedPoints">A set of required points already used to ensure uniqueness.</param>
    /// <param name="userPerformanceId">The ID of the user's performance period.</param>
    private void AssignRewards(RewardLevel rewardLevel, int count, List<ChallengeReward> allRewards, List<UserChallengePeriodReward> userRewards, HashSet<RewardRequiredPoint> usedPoints, int userPerformanceId)
    {
        var availableRewards = allRewards.Where(r => r.Level == rewardLevel).ToList();

        for (int i = 0; i < count; i++)
        {
            var selectedReward = SelectUniqueReward(availableRewards, usedPoints);

            if (selectedReward != null)
            {
                allRewards.Remove(selectedReward); // Remove the selected reward to ensure uniqueness

                userRewards.Add(new UserChallengePeriodReward
                {
                    ChallengeRewardId = selectedReward.Id,
                    UserChallengePeriodPerformanceId = userPerformanceId,
                    ChallengeReward = selectedReward
                });
            }
        }
    }

    /// <summary>
    /// Selects a unique reward from a list of available rewards, ensuring the required points for the reward have not been used before.
    /// </summary>
    /// <param name="availableRewards">A list of available rewards to select from.</param>
    /// <param name="usedPoints">A set of used points to ensure the selected reward is unique.</param>
    /// <returns>A unique reward if found, otherwise null after multiple attempts.</returns>
    private ChallengeReward SelectUniqueReward(List<ChallengeReward> availableRewards, HashSet<RewardRequiredPoint> usedPoints)
    {
        for (int attempt = 0; attempt < 10; attempt++)
        {
            var randomIndex = new Random().Next(availableRewards.Count);
            var reward = availableRewards[randomIndex];

            // Check if the required points are already used
            if (!usedPoints.Contains(reward.RequiredPoints))
            {
                usedPoints.Add(reward.RequiredPoints); // Mark points as used
                return reward;
            }
        }
        return null; // Return null if no unique reward found after 10 attempts
    }
}
