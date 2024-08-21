using DH.Domain.Adapters.Authentication;
using DH.Domain.Models.GameModels.Queries;
using DH.Domain.Services;
using MediatR;

namespace DH.Application.Games.Queries.Games;

public record GetNewGameListQuery(string? SearchExpression) : IRequest<List<GetGameListQueryModel>>;

internal class GetNewGameListQueryHandler : IRequestHandler<GetNewGameListQuery, List<GetGameListQueryModel>>
{
    readonly IGameService gameService;
    readonly IUserContext userContext;

    public GetNewGameListQueryHandler(IGameService gameService, IUserContext userContext)
    {
        this.gameService = gameService;
        this.userContext = userContext;
    }

    public async Task<List<GetGameListQueryModel>> Handle(GetNewGameListQuery request, CancellationToken cancellationToken)
    {
        return await gameService.GetNewGameListBySearchExpressionAsync(request.SearchExpression ?? string.Empty, this.userContext.UserId, cancellationToken);
    }
}
