using DH.Domain.Models.GameModels.Queries;
using DH.Domain.Services;
using MediatR;

namespace DH.Application.Games.Queries.Games;

public record GetGameCategoryListQuery(string? SearchExpression) : IRequest<List<GetGameCategoryListQueryModel>>;

internal class GetGameCategoryListQueryHandler : IRequestHandler<GetGameCategoryListQuery, List<GetGameCategoryListQueryModel>>
{
    readonly IGameCategoryService gameCategoryService;

    public GetGameCategoryListQueryHandler(IGameCategoryService gameCategoryService)
    {
        this.gameCategoryService = gameCategoryService;
    }

    public async Task<List<GetGameCategoryListQueryModel>> Handle(GetGameCategoryListQuery request, CancellationToken cancellationToken)
    {
        return await gameCategoryService.GetListBySearchExpressionAsync(request.SearchExpression ?? string.Empty, cancellationToken);
    }
}
