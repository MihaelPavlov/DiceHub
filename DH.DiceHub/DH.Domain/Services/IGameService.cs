using DH.Domain.Entities;
using DH.Domain.Models.GameModels;
using DH.Domain.Models.GameModels.Queries;

namespace DH.Domain.Services;

public interface IGameService : IDomainService<Game>
{
    Task<List<GameComplexDataQuery>> GetCompexDataAsync(CancellationToken cancellationToken);

    Task<List<GetGameListQueryModel>> GetGameListBySearchExpressionAsync(string searchExpression, CancellationToken cancellationToken);
}
