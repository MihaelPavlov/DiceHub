using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using DH.Domain.Models.ChallengeModels.Queries;
using DH.Domain.Services;
using Microsoft.EntityFrameworkCore;

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
                .OrderByDescending(x=>x.CompletedDate)
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

            return activeChallenges.Union(lastCompletedChallenge).ToList();
        }
    }
}
