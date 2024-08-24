using DH.Domain.Entities;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Games.Commands.Games;

public record DeleteGameCommand(int Id) : IRequest;

internal class DeleteGameCommandHandler : IRequestHandler<DeleteGameCommand>
{
    readonly IRepository<Game> repository;

    public DeleteGameCommandHandler(IRepository<Game> repository)
    {
        this.repository = repository;
    }

    public async Task Handle(DeleteGameCommand request, CancellationToken cancellationToken)
    {
        var game = await this.repository.GetByAsyncWithTracking(x => x.Id == request.Id, cancellationToken)
            ?? throw new Exception("Not Found");

        game.IsDeleted = true;

        await this.repository.Update(game, cancellationToken);
    }
}