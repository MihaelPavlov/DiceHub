using DH.Domain.Adapters.Authentication;
using DH.Domain.Models.GameModels.Queries;
using DH.Domain.Services;
using MediatR;

namespace DH.Application.Games.Queries.Games;

public record GetGameListQuery(string searchExpression) : IRequest<List<GetGameListQueryModel>>;

internal class GetGameListQueryHandler : IRequestHandler<GetGameListQuery, List<GetGameListQueryModel>>
{
    readonly IGameService gameService;
    readonly IUserContext userContext;

    public GetGameListQueryHandler(IGameService gameService, IUserContext userContext)
    {
        this.gameService = gameService;
        this.userContext = userContext;
    }

    public async Task<List<GetGameListQueryModel>> Handle(GetGameListQuery request, CancellationToken cancellationToken)
    {
        return await gameService.GetGameListBySearchExpressionAsync(request.searchExpression, this.userContext.UserId, cancellationToken);
    }
}
