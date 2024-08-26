using DH.Domain.Entities;
using DH.Domain.Models.GameModels;
using DH.Domain.Models.GameModels.Queries;

namespace DH.Domain.Services;

public interface IGameService : IDomainService<Game>
{
    Task<List<GameComplexDataQuery>> GetCompexDataAsync(CancellationToken cancellationToken);
    Task<List<GetGameListQueryModel>> GetGameListBySearchExpressionAsync(string searchExpression, string userId, CancellationToken cancellationToken);
    Task<List<GetGameListQueryModel>> GetNewGameListBySearchExpressionAsync(string searchExpression, string userId, CancellationToken cancellationToken);
    Task<List<GetGameListQueryModel>> GetGameListBySearchExpressionAsync(int categoryId, string searchExpression, string userId, CancellationToken cancellationToken);
    Task<GetGameByIdQueryModel?> GetGameByIdAsync(int gameId, string userId, CancellationToken cancellationToken);
    Task<int> CreateGame(Game game, string fileName, string contentType, MemoryStream imageStream, CancellationToken cancellationToken);
    Task UpdateGame(Game game, string fileName, string contentType, MemoryStream imageStream, CancellationToken cancellationToken);
    Task CreateReservation(GameReservation reservation, CancellationToken cancellationToken);
}
