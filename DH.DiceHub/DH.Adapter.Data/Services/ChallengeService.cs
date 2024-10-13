using DH.Domain.Entities;
using DH.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace DH.Adapter.Data.Services;

public class ChallengeService : IChallengeService
{
    readonly IDbContextFactory<TenantDbContext> _contextFactory;

    public ChallengeService(IDbContextFactory<TenantDbContext> _contextFactory)
    {
        this._contextFactory = _contextFactory;
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
}
