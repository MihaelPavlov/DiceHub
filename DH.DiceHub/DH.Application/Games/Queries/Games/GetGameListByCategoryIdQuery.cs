﻿using DH.Domain.Adapters.Authentication;
using DH.Domain.Models.GameModels.Queries;
using DH.Domain.Services;
using MediatR;

namespace DH.Application.Games.Queries.Games;

public record GetGameListByCategoryIdQuery(int Id, string? SearchExpression) : IRequest<List<GetGameListQueryModel>>;

internal class GetGameListByCategoryIdQueryHandler : IRequestHandler<GetGameListByCategoryIdQuery, List<GetGameListQueryModel>>
{
    readonly IGameService gameService;
    readonly IUserContext userContext;

    public GetGameListByCategoryIdQueryHandler(IGameService gameService, IUserContext userContext)
    {
        this.gameService = gameService;
        this.userContext = userContext;
    }

    public async Task<List<GetGameListQueryModel>> Handle(GetGameListByCategoryIdQuery request, CancellationToken cancellationToken)
    {
        return await gameService.GetGameListBySearchExpressionAsync(request.Id, request.SearchExpression ?? string.Empty, this.userContext.UserId, cancellationToken);
    }
}
