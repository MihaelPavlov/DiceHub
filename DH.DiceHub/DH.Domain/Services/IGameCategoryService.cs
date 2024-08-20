using DH.Domain.Entities;
using DH.Domain.Models.GameModels.Queries;

namespace DH.Domain.Services;

public interface IGameCategoryService : IDomainService<GameCategory>
{
    Task<List<GetGameCategoryListQueryModel>> GetListBySearchExpressionAsync(string searchExpression, CancellationToken cancellationToken);
}
