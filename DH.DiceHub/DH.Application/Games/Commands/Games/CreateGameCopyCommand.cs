using DH.Domain.Entities;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Games.Commands.Games;

public record CreateGameCopyCommand(int Id) : IRequest;

internal class CreateGameCopyCommandHandler : IRequestHandler<CreateGameCopyCommand>
{
    readonly IRepository<Game> repository;

    public CreateGameCopyCommandHandler(IRepository<Game> repository)
    {
        this.repository = repository;
    }

    public async Task Handle(CreateGameCopyCommand request, CancellationToken cancellationToken)
    {
        var game = await this.repository.GetByAsyncWithTracking(x => x.Id == request.Id, cancellationToken)
            ?? throw new Exception("Not Found");

        game.CopyCount++;

        await this.repository.Update(game, cancellationToken);
    }
}