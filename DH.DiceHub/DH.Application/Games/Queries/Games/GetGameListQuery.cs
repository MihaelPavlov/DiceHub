using DH.Application.Cqrs;
using DH.Domain.Cqrs;
using DH.Domain.Models.GameModels.Queries;
using DH.Domain.Services;

namespace DH.Application.Games.Queries.Games;

public record GetGameListQuery(string searchExpression) : ICommand<List<GetGameListQueryModel>>;

internal class GetGameListQueryHandler : AbstractCommandHandler<GetGameListQuery, List<GetGameListQueryModel>>
{
    readonly IGameService gameService;

    public GetGameListQueryHandler(IGameService gameService)
    {
        this.gameService = gameService;
    }

    protected override async Task<List<GetGameListQueryModel>> HandleAsync(GetGameListQuery request, CancellationToken cancellationToken)
    {
        return await gameService.GetGameListBySearchExpressionAsync(request.searchExpression, cancellationToken);
    }
}
