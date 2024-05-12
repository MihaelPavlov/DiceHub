using DH.Domain.Models.GameModels;
using DH.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace DH.Adapter.Data.Services;

public class GameService : IGameService
{
    readonly IDbContextFactory<TenantDbContext> _contextFactory;

    public GameService(IDbContextFactory<TenantDbContext> _contextFactory)
    {
        this._contextFactory = _contextFactory;
    }

    public async Task<List<GameComplexDataQuery>> GetCompexDataAsync(CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var games = await (from g in context.Games
                               where g.Name == "string"
                               select new GameComplexDataQuery
                               {
                                   Id = g.Id,
                                   Name = g.Name
                               }).ToListAsync(cancellationToken);

            return games;
        }

    }
}
