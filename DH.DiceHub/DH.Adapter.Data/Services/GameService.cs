using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.FileManager;
using DH.Domain.Adapters.Localization;
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
    readonly TenantDbContext tenantDbContext;
    readonly ITenantSettingsCacheService tenantSettingsCacheService;
    readonly IUserContext userContext;
    readonly IFileManagerClient fileManagerClient;
    readonly ILocalizationService localizationService;

    public GameService(
        IDbContextFactory<TenantDbContext> contextFactory,
        TenantDbContext tenantDbContext,
        ITenantSettingsCacheService tenantSettingsCacheService,
        IUserContext userContext,
        IFileManagerClient fileManagerClient,
        ILocalizationService localizationService)
    {
        this.contextFactory = contextFactory;
        this.tenantDbContext = tenantDbContext;
        this.tenantSettingsCacheService = tenantSettingsCacheService;
        this.userContext = userContext;
        this.fileManagerClient = fileManagerClient;
        this.localizationService = localizationService;
    }

    public async Task<int> CreateGame(Game game, string fileName, string contentType, MemoryStream imageStream, CancellationToken cancellationToken)
    {
        var imageUrl = await this.fileManagerClient.UploadFileAsync(
            FileManagerFolders.Games.ToString(), fileName, imageStream.ToArray());

        using (var context = await contextFactory.CreateDbContextAsync(cancellationToken))
        {
            game.ImageUrl = imageUrl;
            await context.Games.AddAsync(game, cancellationToken);

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

    public async Task UpdateGame(Game game, string? fileName, string? contentType, MemoryStream? imageStream, CancellationToken cancellationToken)
    {
        string? currenFileImageUrl = null;

        if (!string.IsNullOrEmpty(fileName))
            currenFileImageUrl = this.fileManagerClient.GetPublicUrl(
                FileManagerFolders.Games.ToString(), fileName);

        using (var context = await contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var dbGame = await context.Games
                .AsTracking()
                .FirstOrDefaultAsync(x => x.Id == game.Id, cancellationToken)
                    ?? throw new NotFoundException(nameof(Game), game.Id);

            dbGame.Name = game.Name;
            dbGame.Description_EN = game.Description_EN;
            dbGame.Description_BG = game.Description_BG;
            dbGame.MinAge = game.MinAge;
            dbGame.MinPlayers = game.MinPlayers;
            dbGame.MaxPlayers = game.MaxPlayers;
            dbGame.AveragePlaytime = game.AveragePlaytime;
            dbGame.UpdatedDate = DateTime.UtcNow;
            dbGame.CategoryId = game.CategoryId;
            if (currenFileImageUrl != null && currenFileImageUrl != dbGame.ImageUrl && imageStream != null)
            {
                var imageUrl = await this.fileManagerClient.UploadFileAsync(
                    FileManagerFolders.Games.ToString(), fileName!, imageStream.ToArray());
                dbGame.ImageUrl = imageUrl;
            }

            await context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<List<GetActiveGameReservationListQueryModel>> GetActiveGameReservation(CancellationToken cancellationToken)
    {
        using (var context = await contextFactory.CreateDbContextAsync(cancellationToken))
        {
            return await (
                from gameReservation in context.GameReservations.AsNoTracking()
                where gameReservation.IsActive
                orderby gameReservation.ReservationDate descending
                let tableReservation = context.SpaceTableReservations.AsNoTracking()
                    .Where(t => t.IsActive && t.UserId == gameReservation.UserId && gameReservation.ReservationDate.Date == t.ReservationDate.Date)
                    .FirstOrDefault()
                select new GetActiveGameReservationListQueryModel
                {
                    Id = gameReservation.Id,
                    GameId = gameReservation.Game.Id,
                    GameName = gameReservation.Game.Name,
                    GameImageUrl = gameReservation.Game.ImageUrl,
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
                where g.Id == gameId && !g.IsDeleted
                select new GetGameByIdQueryModel
                {
                    Id = g.Id,
                    CategoryId = g.CategoryId,
                    Name = g.Name,
                    Description_EN = g.Description_EN,
                    Description_BG = g.Description_BG,
                    AveragePlaytime = g.AveragePlaytime,
                    ImageUrl = g.ImageUrl,
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
        var context = this.tenantDbContext;

        return await
                (from g in context.Games
                 where !g.IsDeleted && g.Name.ToLower().Contains(searchExpression.ToLower())
                 let likes = context.GameLikes.Where(x => x.GameId == g.Id).ToList()
                 select new GetGameListQueryModel
                 {
                     Id = g.Id,
                     CategoryId = g.CategoryId,
                     Name = g.Name,
                     Description_EN = g.Description_EN,
                     Description_BG = g.Description_BG,
                     ImageUrl = g.ImageUrl,
                     Likes = likes.Count(),
                     IsLiked = likes.Any(x => x.UserId == userId)
                 })
                 .OrderBy(x => x.Name)
                 .ToListAsync(cancellationToken);
    }

    public async Task<List<GetGameListQueryModel>> GetGameListBySearchExpressionAsync(int categoryId, string searchExpression, string userId, CancellationToken cancellationToken)
    {
        using (var context = await contextFactory.CreateDbContextAsync(cancellationToken))
        {
            using (var transaction = await context.Database.BeginTransactionAsync())
            {

                return await
                (from g in context.Games
                 where !g.IsDeleted && g.Name.ToLower().Contains(searchExpression.ToLower()) && g.CategoryId == categoryId
                 let likes = context.GameLikes.Where(x => x.GameId == g.Id).ToList()
                 select new GetGameListQueryModel
                 {
                     Id = g.Id,
                     CategoryId = g.CategoryId,
                     Name = g.Name,
                     Description_EN = g.Description_EN,
                     Description_BG = g.Description_BG,
                     ImageUrl = g.ImageUrl,
                     Likes = likes.Count(),
                     IsLiked = likes.Any(x => x.UserId == userId)
                 })
                 .OrderBy(x => x.Name)
                 .ToListAsync(cancellationToken);
            }
        }
    }

    public async Task<List<GetGameListQueryModel>> GetNewGameListBySearchExpressionAsync(string searchExpression, string userId, CancellationToken cancellationToken)
    {
        using (var context = await contextFactory.CreateDbContextAsync(cancellationToken))
        {
            return await
                (from g in context.Games
                 where !g.IsDeleted && g.Name.ToLower().Contains(searchExpression.ToLower()) && g.CreatedDate >= DateTime.UtcNow.AddDays(-7)
                 let likes = context.GameLikes.Where(x => x.GameId == g.Id).ToList()
                 select new GetGameListQueryModel
                 {
                     Id = g.Id,
                     CategoryId = g.CategoryId,
                     Name = g.Name,
                     Description_EN = g.Description_EN,
                     Description_BG = g.Description_BG,
                     ImageUrl = g.ImageUrl,
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
                throw new ValidationErrorsException("ActiveGameReservations", this.localizationService["DeleteGameActiveReservationWarning"]);

            if (dbGameReservations.Count > 1)
                throw new ValidationErrorsException("ActiveGameReservations", this.localizationService["DeleteGameMultipleActiveReservationsWarning"]);

            var today = DateTime.UtcNow;
            var dbEvents = await context.Events
                .Where(x => x.GameId == id && x.StartDate > today)
                .ToListAsync(cancellationToken);

            if (dbEvents.Count == 1)
                throw new ValidationErrorsException("ActiveEvents", this.localizationService["DeleteGameLinkedUpcomingEventWarning"]);

            if (dbEvents.Count > 1)
                throw new ValidationErrorsException("ActiveEvents", this.localizationService["DeleteGameLinkedMultipleUpcomingEventsWarning"]);

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
                    throw new ValidationErrorsException("ActiveCustomPeriod", this.localizationService["DeleteGameActiveCustomChallengePeriodWarning"]);
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
                    throw new ValidationErrorsException("ActiveChallenges", this.localizationService["DeleteGameActiveChallengeWarning"]);
            }

            dbGame.IsDeleted = true;

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
