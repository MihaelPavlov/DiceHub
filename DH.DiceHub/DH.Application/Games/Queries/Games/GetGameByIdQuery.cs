using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Models.GameModels.Queries;
using DH.Domain.Services;
using DH.Messaging.Publisher;
using MediatR;

namespace DH.Application.Games.Queries.Games;

public record GetGameByIdQuery(int Id) : IRequest<GetSystemRewardByIdQueryModel>;

internal class GetGameByIdQueryHandler : IRequestHandler<GetGameByIdQuery, GetSystemRewardByIdQueryModel>
{
    readonly IGameService gameService;
    readonly IUserContext userContext;
    readonly IRabbitMqClient _rabbitMqClient;

    public GetGameByIdQueryHandler(IGameService gameService, IUserContext userContext, IRabbitMqClient rabbitMqClient)
    {
        this.gameService = gameService;
        this.userContext = userContext;
        this._rabbitMqClient = rabbitMqClient;
    }

    public async Task<GetSystemRewardByIdQueryModel> Handle(GetGameByIdQuery request, CancellationToken cancellationToken)
    {
        return await this.gameService.GetGameByIdAsync(request.Id, userContext.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(Game), request.Id);
    }
}
