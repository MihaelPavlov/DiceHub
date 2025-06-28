using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Models.ChallengeModels.Queries;
using DH.Domain.Services;
using DH.OperationResultCore.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DH.Adapter.Data.Services;

public class ChallengeService : IChallengeService
{
    readonly IDbContextFactory<TenantDbContext> _contextFactory;
    readonly IUserContext userContext;

    public ChallengeService(IDbContextFactory<TenantDbContext> _contextFactory, IUserContext userContext)
    {
        this._contextFactory = _contextFactory;
        this.userContext = userContext;
    }

    public async Task<int> Create(Challenge challenge, CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            await context.ChallengeStatistics.AddAsync(new ChallengeStatistic
            {
                Challenge = challenge,
                TotalCompletions = 0
            }, cancellationToken);

            await context.SaveChangesAsync(cancellationToken);

            return challenge.Id;
        }
    }

    public async Task Delete(int id, CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var challenge = await context.Challenges
                .Include(x => x.UserChallenges)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (challenge == null)
                throw new NotFoundException(nameof(Challenge), id);

            if (challenge.UserChallenges.Count != 0)
                throw new ValidationErrorsException("UserChallenges", "Challenge has dependencies and cannot be deleted");

            context.Challenges.Remove(challenge);

            await context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<List<GetUserChallengeListQueryModel>> GetUserChallenges(CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var activeChallenges = await context.UserChallenges
                .Where(x => this.userContext.UserId == x.UserId && x.IsActive)
                .Select(x =>
                    new GetUserChallengeListQueryModel
                    {
                        Id = x.Id,
                        Description = x.Challenge.Description,
                        RewardPoints = x.Challenge.RewardPoints,
                        Status = x.Status,
                        GameImageId = x.Challenge.Game.Image.Id,
                        GameName = x.Challenge.Game.Name,
                        CurrentAttempts = x.AttemptCount,
                        MaxAttempts = x.Challenge.Attempts,
                    })
                .ToListAsync(cancellationToken);

            var lastCompletedChallenge = await context.UserChallenges
                .Where(x => this.userContext.UserId == x.UserId && x.CompletedDate != null)
                .OrderByDescending(x => x.CompletedDate)
                .Select(x =>
                        new GetUserChallengeListQueryModel
                        {
                            Id = x.Id,
                            Description = x.Challenge.Description,
                            RewardPoints = x.Challenge.RewardPoints,
                            Status = x.Status,
                            GameImageId = x.Challenge.Game.Image.Id,
                            GameName = x.Challenge.Game.Name,
                            CurrentAttempts = x.AttemptCount,
                            MaxAttempts = x.Challenge.Attempts,
                        })
                .FirstOrDefaultAsync(cancellationToken);

            var orderedChallenges = activeChallenges
                .OrderBy(x => x.Status)
                .ThenBy(x => x.RewardPoints)
                .ToList();

            if (lastCompletedChallenge != null)
                orderedChallenges.Add(lastCompletedChallenge);

            return orderedChallenges;
        }
    }
}
