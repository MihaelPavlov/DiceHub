using DH.Domain.Models.GameModels;
using DH.Domain.Models.GameModels.Queries;
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

    public async Task<GetGameByIdQueryModel?> GetGameByIdAsync(int gameId, string userId, CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            return await context.Games
                .Include(x => x.Likes)
                .Where(x => x.Id == gameId)
                .Select(game => new GetGameByIdQueryModel
                {
                    Id = game.Id,
                    Name = game.Name,
                    Description = game.Description,
                    AveragePlaytime = game.AveragePlaytime,
                    ImageUrl = game.ImageUrl,
                    Likes = game.Likes.Count(),
                    IsLiked = game.Likes.Any(x => x.UserId == userId),
                    MinAge = game.MinAge,
                    MaxPlayers = game.MaxPlayers,
                    MinPlayers = game.MinPlayers,
                })
                .FirstOrDefaultAsync();
        }
    }

    public async Task<List<GetGameListQueryModel>> GetGameListBySearchExpressionAsync(string searchExpression, string userId, CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            return await (from g in context.Games
                          where g.Name.ToLower().Contains(searchExpression.ToLower())
                          let likes = context.GameLikes.Where(x => x.GameId == g.Id).ToList()
                          select new GetGameListQueryModel
                          {
                              Id = g.Id,
                              Name = g.Name,
                              Description = g.Description,
                              ImageUrl = g.ImageUrl,
                              Likes = likes.Count(),
                              IsLiked = likes.Any(x => x.UserId == userId)
                          }).ToListAsync(cancellationToken);
        }
    }
}
