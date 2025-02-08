using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Models.GameModels.Queries;

namespace DH.Domain.Services;

public interface IGameService : IDomainService<Game>
{
    Task<List<GetActiveGameReservationListQueryModel>> GetActiveGameReservation(CancellationToken cancellationToken);
    Task<List<GetGameListQueryModel>> GetGameListBySearchExpressionAsync(string searchExpression, string userId, CancellationToken cancellationToken);
    Task<List<GetGameListQueryModel>> GetNewGameListBySearchExpressionAsync(string searchExpression, string userId, CancellationToken cancellationToken);
    Task<List<GetGameListQueryModel>> GetGameListBySearchExpressionAsync(int categoryId, string searchExpression, string userId, CancellationToken cancellationToken);
    Task<GetSystemRewardByIdQueryModel?> GetGameByIdAsync(int gameId, string userId, CancellationToken cancellationToken);
    Task<int> CreateGame(Game game, string fileName, string contentType, MemoryStream imageStream, CancellationToken cancellationToken);
    Task UpdateGame(Game game, string fileName, string contentType, MemoryStream imageStream, CancellationToken cancellationToken);
    Task CreateReservation(GameReservation reservation, CancellationToken cancellationToken);
    Task<List<GetGameReservationHistoryQueryModel>> GetGameReservationListByStatus(ReservationStatus? status, CancellationToken cancellationToken);
    Task<int> GetActiveGameReservationsCount(CancellationToken cancellationToken);
}
