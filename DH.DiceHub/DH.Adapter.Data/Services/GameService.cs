using DH.Domain.Entities;
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

    public async Task<int> CreateGame(Game game, string fileName, string contentType, MemoryStream imageStream, CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var gameImage = new GameImage
            {
                FileName = fileName,
                ContentType = contentType,
                Data = imageStream.ToArray(),
            };

            await context.GameImages.AddAsync(gameImage, cancellationToken);

            game.Image = gameImage;

            await context.Games.AddAsync(game, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return game.Id;
        }
    }

    public async Task UpdateGame(Game game, string fileName, string contentType, MemoryStream imageStream, CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var dbGame = await context.Games
                .AsTracking()
                .Include(g => g.Image)
                .FirstOrDefaultAsync(x => x.Id == game.Id, cancellationToken)
                    ?? throw new Exception("Not Found");

            var oldImage = dbGame.Image;

            dbGame.Name = game.Name;
            dbGame.Description = game.Description;
            dbGame.MinAge = game.MinAge;
            dbGame.MinPlayers = game.MinPlayers;
            dbGame.MaxPlayers = game.MaxPlayers;
            dbGame.AveragePlaytime = game.AveragePlaytime;
            dbGame.CreatedDate = DateTime.UtcNow;
            dbGame.UpdatedDate = DateTime.UtcNow;
            dbGame.CategoryId = game.CategoryId;

            var newGameImage = new GameImage
            {
                FileName = fileName,
                ContentType = contentType,
                Data = imageStream.ToArray(),
            };

            dbGame.Image = newGameImage;

            context.GameImages.Remove(oldImage);

            await context.SaveChangesAsync(cancellationToken);
        }
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
                    CategoryId = game.CategoryId,
                    Name = game.Name,
                    Description = game.Description,
                    AveragePlaytime = game.AveragePlaytime,
                    ImageId = game.ImageId,
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
            return await
                (from g in context.Games
                 where g.Name.ToLower().Contains(searchExpression.ToLower())
                 let likes = context.GameLikes.Where(x => x.GameId == g.Id).ToList()
                 select new GetGameListQueryModel
                 {
                     Id = g.Id,
                     CategoryId = g.CategoryId,
                     Name = g.Name,
                     Description = g.Description,
                     ImageId = g.ImageId,
                     Likes = likes.Count(),
                     IsLiked = likes.Any(x => x.UserId == userId)
                 }).ToListAsync(cancellationToken);
        }
    }

    public async Task<List<GetGameListQueryModel>> GetGameListBySearchExpressionAsync(int categoryId, string searchExpression, string userId, CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            return await
                (from g in context.Games
                 where g.Name.ToLower().Contains(searchExpression.ToLower()) && g.CategoryId == categoryId
                 let likes = context.GameLikes.Where(x => x.GameId == g.Id).ToList()
                 select new GetGameListQueryModel
                 {
                     Id = g.Id,
                     CategoryId = g.CategoryId,
                     Name = g.Name,
                     Description = g.Description,
                     ImageId = g.ImageId,
                     Likes = likes.Count(),
                     IsLiked = likes.Any(x => x.UserId == userId)
                 }).ToListAsync(cancellationToken);
        }
    }

    public async Task<List<GetGameListQueryModel>> GetNewGameListBySearchExpressionAsync(string searchExpression, string userId, CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            return await
                (from g in context.Games
                 where g.Name.ToLower().Contains(searchExpression.ToLower()) && g.CreatedDate >= DateTime.UtcNow.AddDays(-7)
                 let likes = context.GameLikes.Where(x => x.GameId == g.Id).ToList()
                 select new GetGameListQueryModel
                 {
                     Id = g.Id,
                     CategoryId = g.CategoryId,
                     Name = g.Name,
                     Description = g.Description,
                     ImageId = g.ImageId,
                     Likes = likes.Count(),
                     IsLiked = likes.Any(x => x.UserId == userId)
                 }).ToListAsync(cancellationToken);
        }
    }
}
