using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using DH.Domain.Models.GameModels.Queries;
using DH.Domain.Services;
using DH.OperationResultCore.Exceptions;
using MediatR;

namespace DH.Application.Games.Queries.Games;

public record GetGameByIdQuery(int Id) : IRequest<GetGameByIdQueryModel>;

internal class GetGameByIdQueryHandler : IRequestHandler<GetGameByIdQuery, GetGameByIdQueryModel>
{
    readonly IGameService gameService;
    readonly IUserContext userContext;

    public GetGameByIdQueryHandler(IGameService gameService, IUserContext userContext)
    {
        this.gameService = gameService;
        this.userContext = userContext;
    }

    public async Task<GetGameByIdQueryModel> Handle(GetGameByIdQuery request, CancellationToken cancellationToken)
    {
        return await this.gameService.GetGameByIdAsync(request.Id, userContext.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(Game), request.Id);
    }
}
