using DH.Domain.Models.GameModels.Queries;
using DH.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace DH.Adapter.Data.Services;

public class GameCategoryService : IGameCategoryService
{
    readonly IDbContextFactory<TenantDbContext> _contextFactory;

    public GameCategoryService(IDbContextFactory<TenantDbContext> _contextFactory)
    {
        this._contextFactory = _contextFactory;
    }

    public async Task<List<GetGameCategoryListQueryModel>> GetListBySearchExpressionAsync(string searchExpression, CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            return await
                (from g in context.GameCategories.AsNoTracking()
                 where g.Name.ToLower().Contains(searchExpression.ToLower())
                 select new GetGameCategoryListQueryModel
                 {
                     Id = g.Id,
                     Name = g.Name,
                 })
                 .OrderBy(x => x.Name)
                 .ToListAsync(cancellationToken);
        }
    }
}
