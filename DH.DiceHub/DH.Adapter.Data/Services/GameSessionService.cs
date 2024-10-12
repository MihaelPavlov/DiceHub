
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Exceptions;
using DH.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace DH.Adapter.Data.Services;

/// <inheritdoc/>
public class GameSessionService : IGameSessionService
{
    readonly IDbContextFactory<TenantDbContext> dbContextFactory;

    public GameSessionService(IDbContextFactory<TenantDbContext> dbContextFactory)
    {
        this.dbContextFactory = dbContextFactory;
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
                    //TODO: Check if we gonna need include(x=>x.Challenges)
                    var userChallenges = await context.UserChallenges
                    .AsTracking()
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

                        var challengeStats = challengeStatistics.First(x => x.ChallengeId == challenge.ChallengeId);
                        challengeStats.TotalCompletions++;
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
    public async Task<bool> EvaluateRewardsAfterChallenges(string userId, CancellationToken cancellationToken)
    {
        using (var context = await this.dbContextFactory.CreateDbContextAsync(cancellationToken))
        {
            using (var transaction = await context.Database.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    var userChallenges = await context.UserChallenges
                        .AsTracking()
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
}
