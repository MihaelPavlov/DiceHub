using DH.Domain.Adapters.Authentication;
using DH.Domain.Models.GameModels.Queries;
using DH.Domain.Services;
using MediatR;

namespace DH.Application.Games.Queries.Games;

public record GetGameListQuery(string? SearchExpression) : IRequest<List<GetGameListQueryModel>>;

internal class GetGameListQueryHandler(IGameService gameService, IUserContext userContext) : IRequestHandler<GetGameListQuery, List<GetGameListQueryModel>>
{
    readonly IGameService gameService = gameService;
    readonly IUserContext userContext = userContext;

    public async Task<List<GetGameListQueryModel>> Handle(GetGameListQuery request, CancellationToken cancellationToken)
    {
        return await gameService.GetGameListBySearchExpressionAsync(request.SearchExpression ?? string.Empty, this.userContext.UserId, cancellationToken);
    }
}
