using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Models.GameModels.Queries;
using DH.Domain.Services;
using DH.Domain.Services.TenantSettingsService;
using DH.OperationResultCore.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DH.Adapter.Data.Services;

public class GameService : IGameService
{
    readonly IDbContextFactory<TenantDbContext> contextFactory;
    readonly ITenantSettingsCacheService tenantSettingsCacheService;
    readonly IUserContext userContext;

    public GameService(
        IDbContextFactory<TenantDbContext> contextFactory,
        ITenantSettingsCacheService tenantSettingsCacheService,
        IUserContext userContext)
    {
        this.contextFactory = contextFactory;
        this.tenantSettingsCacheService = tenantSettingsCacheService;
        this.userContext = userContext;
    }

    public async Task<int> CreateGame(Game game, string fileName, string contentType, MemoryStream imageStream, CancellationToken cancellationToken)
    {
        using (var context = await contextFactory.CreateDbContextAsync(cancellationToken))
        {
            await context.Games.AddAsync(game, cancellationToken);

            await context.GameImages
                .AddAsync(new GameImage
                {
                    Game = game,
                    FileName = fileName,
                    ContentType = contentType,
                    Data = imageStream.ToArray(),
                }, cancellationToken);

            await context.GameInventories
                .AddAsync(new GameInventory
                {
                    Game = game,
                    AvailableCopies = 1,
                    TotalCopies = 1,
                }, cancellationToken);

            await context.SaveChangesAsync(cancellationToken);

            return game.Id;
        }
    }

    public async Task UpdateGame(Game game, string fileName, string contentType, MemoryStream imageStream, CancellationToken cancellationToken)
    {
        using (var context = await contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var dbGame = await context.Games
                .AsTracking()
                .Include(g => g.Image)
                .FirstOrDefaultAsync(x => x.Id == game.Id, cancellationToken)
                    ?? throw new NotFoundException(nameof(Game), game.Id);

            var oldImage = dbGame.Image;

            dbGame.Name = game.Name;
            dbGame.Description_EN = game.Description_EN;
            dbGame.Description_BG = game.Description_BG;
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

    public async Task<List<GetActiveGameReservationListQueryModel>> GetActiveGameReservation(CancellationToken cancellationToken)
    {
        using (var context = await contextFactory.CreateDbContextAsync(cancellationToken))
        {
            return await (
                from gameReservation in context.GameReservations
                where gameReservation.IsActive
                orderby gameReservation.ReservationDate descending
                let tableReservation = context.SpaceTableReservations
                    .Where(t => t.IsActive && t.UserId == gameReservation.UserId && gameReservation.ReservationDate.Date == t.ReservationDate.Date)
                    .FirstOrDefault()
                select new GetActiveGameReservationListQueryModel
                {
                    Id = gameReservation.Id,
                    GameId = gameReservation.Game.Id,
                    GameName = gameReservation.Game.Name,
                    GameImageId = gameReservation.Game.Image.Id,
                    CreatedDate = gameReservation.CreatedDate,
                    ReservationDate = gameReservation.ReservationDate,
                    ReservedDurationMinutes = gameReservation.ReservedDurationMinutes,
                    Status = gameReservation.Status,
                    UserId = gameReservation.UserId,
                    NumberOfGuests = gameReservation.NumberOfGuests,
                    UserHaveActiveTableReservation = tableReservation != null,
                    TableReservationTime = tableReservation != null
                        ? tableReservation.ReservationDate
                        : null
                }).ToListAsync();
        }
    }

    public async Task<GetGameByIdQueryModel?> GetGameByIdAsync(int gameId, string userId, CancellationToken cancellationToken)
    {
        using (var context = await contextFactory.CreateDbContextAsync(cancellationToken))
        {
            return await (
                from g in context.Games
                join gi in context.GameImages on g.Id equals gi.GameId
                where g.Id == gameId && !g.IsDeleted
                select new GetGameByIdQueryModel
                {
                    Id = g.Id,
                    CategoryId = g.CategoryId,
                    Name = g.Name,
                    Description_EN = g.Description_EN,
                    Description_BG = g.Description_BG,
                    AveragePlaytime = g.AveragePlaytime,
                    ImageId = gi.Id,
                    Likes = g.Likes.Count(),
                    IsLiked = g.Likes.Any(x => x.UserId == userId),
                    MinAge = g.MinAge,
                    MaxPlayers = g.MaxPlayers,
                    MinPlayers = g.MinPlayers,
                }).FirstOrDefaultAsync(cancellationToken);
        }
    }

    public async Task<List<GetGameListQueryModel>> GetGameListBySearchExpressionAsync(string searchExpression, string userId, CancellationToken cancellationToken)
    {
        using (var context = await contextFactory.CreateDbContextAsync(cancellationToken))
        {
            return await
                (from g in context.Games
                 join gi in context.GameImages on g.Id equals gi.GameId
                 where !g.IsDeleted && g.Name.ToLower().Contains(searchExpression.ToLower())
                 let likes = context.GameLikes.Where(x => x.GameId == g.Id).ToList()
                 select new GetGameListQueryModel
                 {
                     Id = g.Id,
                     CategoryId = g.CategoryId,
                     Name = g.Name,
                     Description_EN = g.Description_EN,
                     Description_BG = g.Description_BG,
                     ImageId = gi.Id,
                     Likes = likes.Count(),
                     IsLiked = likes.Any(x => x.UserId == userId)
                 })
                 .OrderBy(x => x.Name)
                 .ToListAsync(cancellationToken);
        }
    }

    public async Task<List<GetGameListQueryModel>> GetGameListBySearchExpressionAsync(int categoryId, string searchExpression, string userId, CancellationToken cancellationToken)
    {
        using (var context = await contextFactory.CreateDbContextAsync(cancellationToken))
        {
            return await
                (from g in context.Games
                 join gi in context.GameImages on g.Id equals gi.GameId
                 where !g.IsDeleted && g.Name.ToLower().Contains(searchExpression.ToLower()) && g.CategoryId == categoryId
                 let likes = context.GameLikes.Where(x => x.GameId == g.Id).ToList()
                 select new GetGameListQueryModel
                 {
                     Id = g.Id,
                     CategoryId = g.CategoryId,
                     Name = g.Name,
                     Description_EN = g.Description_EN,
                     Description_BG = g.Description_BG,
                     ImageId = gi.Id,
                     Likes = likes.Count(),
                     IsLiked = likes.Any(x => x.UserId == userId)
                 })
                 .OrderBy(x => x.Name)
                 .ToListAsync(cancellationToken);
        }
    }

    public async Task<List<GetGameListQueryModel>> GetNewGameListBySearchExpressionAsync(string searchExpression, string userId, CancellationToken cancellationToken)
    {
        using (var context = await contextFactory.CreateDbContextAsync(cancellationToken))
        {
            return await
                (from g in context.Games
                 join gi in context.GameImages on g.Id equals gi.GameId
                 where !g.IsDeleted && g.Name.ToLower().Contains(searchExpression.ToLower()) && g.CreatedDate >= DateTime.UtcNow.AddDays(-7)
                 let likes = context.GameLikes.Where(x => x.GameId == g.Id).ToList()
                 select new GetGameListQueryModel
                 {
                     Id = g.Id,
                     CategoryId = g.CategoryId,
                     Name = g.Name,
                     Description_EN = g.Description_EN,
                     Description_BG = g.Description_BG,
                     ImageId = gi.Id,
                     Likes = likes.Count(),
                     IsLiked = likes.Any(x => x.UserId == userId)
                 })
                 .OrderBy(x => x.Name)
                 .ToListAsync(cancellationToken);
        }
    }

    public async Task CreateReservation(GameReservation reservation, CancellationToken cancellationToken)
    {
        using (var context = await contextFactory.CreateDbContextAsync(cancellationToken))
        {
            await context.GameReservations.AddAsync(reservation, cancellationToken);
            var inventory = await context.GameInventories.AsTracking().FirstOrDefaultAsync(x => x.GameId == reservation.GameId, cancellationToken)
                ?? throw new NotFoundException(nameof(GameInventory), reservation.GameId);

            if (inventory.AvailableCopies > 0)
            {
                inventory.AvailableCopies--;
            }

            await context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<List<GetGameReservationHistoryQueryModel>> GetGameReservationListByStatus(ReservationStatus? status, CancellationToken cancellationToken)
    {
        using (var context = await contextFactory.CreateDbContextAsync(cancellationToken))
        {
            IQueryable<GameReservation> query = context.GameReservations;

            query = status switch
            {
                ReservationStatus.Pending => query.Where(x => x.Status == ReservationStatus.Pending),
                ReservationStatus.Expired => query.Where(x => x.Status == ReservationStatus.Expired),
                ReservationStatus.Accepted => query.Where(x => x.Status == ReservationStatus.Accepted),
                ReservationStatus.Declined => query.Where(x => x.Status == ReservationStatus.Declined),
                _ => query
            };

            return await (
                from x in query
                let tableReservation = context.SpaceTableReservations
                  .Where(t => t.IsActive && t.UserId == x.UserId && x.ReservationDate.Date == t.ReservationDate.Date)
                  .FirstOrDefault()
                select new GetGameReservationHistoryQueryModel
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    CreatedDate = x.CreatedDate,
                    GameId = x.Game.Id,
                    GameName = x.Game.Name,
                    ReservationDate = x.ReservationDate,
                    NumberOfGuests = x.NumberOfGuests,
                    IsActive = x.IsActive,
                    IsReservationSuccessful = x.IsReservationSuccessful,
                    Status = x.Status,
                    UserHaveActiveTableReservation = tableReservation != null,
                    TableReservationTime = tableReservation != null
                      ? tableReservation.ReservationDate
                      : null
                })
                .OrderByDescending(x => x.CreatedDate)
                .OrderBy(x => x.Status == ReservationStatus.Accepted || x.Status == ReservationStatus.Declined)
                .ThenByDescending(x => x.ReservationDate)
                .ToListAsync(cancellationToken);
        }
    }

    public async Task<int> GetActiveGameReservationsCount(CancellationToken cancellationToken)
    {
        using (var context = await contextFactory.CreateDbContextAsync(cancellationToken))
        {
            return await context.GameReservations.Where(x => x.IsActive).CountAsync(cancellationToken);
        }
    }

    public async Task DeleteGame(int id, CancellationToken cancellationToken)
    {
        using (var context = await contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var dbGame = await context.Games
                 .AsTracking()
                 .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
                     ?? throw new NotFoundException(nameof(Game), id);

            var dbGameReservations = await context.GameReservations
                .Where(x => x.GameId == id && x.IsActive)
                .ToListAsync(cancellationToken);

            if (dbGameReservations.Count == 1)
                throw new ValidationErrorsException("ActiveGameReservations",
                    "This game has one active reservation. Please close it before deleting the game.");

            if (dbGameReservations.Count > 1)
                throw new ValidationErrorsException("ActiveGameReservations",
                    "This game has multiple active reservations. Please close them before deleting the game.");

            var today = DateTime.UtcNow;
            var dbEvents = await context.Events
                .Where(x => x.GameId == id && x.StartDate > today)
                .ToListAsync(cancellationToken);

            if (dbEvents.Count == 1)
                throw new ValidationErrorsException("ActiveEvents",
                    "This game is linked to one upcoming event. Cancel the event before deleting the game.");

            if (dbEvents.Count > 1)
                throw new ValidationErrorsException("ActiveEvents",
                    "This game is linked to multiple upcoming events. Cancel them before deleting the game.");

            var tenantSettings = await this.tenantSettingsCacheService.GetGlobalTenantSettingsAsync(cancellationToken);

            if (tenantSettings.IsCustomPeriodOn)
            {
                var customPeriods = await context.UserChallengePeriodPerformances
                    .AsTracking()
                    .Include(x => x.CustomPeriodUserChallenges)
                    .ThenInclude(x => x.Game)
                    .Where(x => x.UserId == this.userContext.UserId && x.IsPeriodActive)
                    .ToListAsync(cancellationToken);

                var gamesInCustomPeriods = customPeriods
                    .SelectMany(x => x.CustomPeriodUserChallenges)
                    .Where(x => x.GameId == id)
                    .ToList();

                if (gamesInCustomPeriods.Any())
                    throw new ValidationErrorsException("ActiveCustomPeriod",
                        "This game is part of an active custom challenge period. Wait to end the period before deleting the game.");
            }
            else
            {
                var userChallenges = await context.UserChallenges
                    .AsTracking()
                    .Include(x => x.Challenge)
                    .ThenInclude(x => x.Game)
                    .Where(x =>
                        x.UserId == this.userContext.UserId &&
                        x.IsActive &&
                        x.Challenge.GameId == id)
                    .ToListAsync(cancellationToken);

                var gamesInActiveChallenges = userChallenges
                    .Where(x => x.Challenge.GameId == id)
                    .ToList();

                if (gamesInActiveChallenges.Any())
                    throw new ValidationErrorsException("ActiveChallenges",
                        "This game is part of an active challenge. Please wait until you receive challenges not linked to this game, or switch to a custom period before deleting it.";
            }

            dbGame.IsDeleted = true;

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
