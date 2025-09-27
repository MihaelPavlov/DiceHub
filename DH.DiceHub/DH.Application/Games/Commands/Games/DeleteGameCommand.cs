using DH.Domain.Services;
using MediatR;

namespace DH.Application.Games.Commands.Games;

public record DeleteGameCommand(int Id) : IRequest;

internal class DeleteGameCommandHandler(IGameService gameService) : IRequestHandler<DeleteGameCommand>
{
    readonly IGameService gameService = gameService;

    public async Task Handle(DeleteGameCommand request, CancellationToken cancellationToken)
    {
        await this.gameService.DeleteGame(request.Id, cancellationToken);
    }
}