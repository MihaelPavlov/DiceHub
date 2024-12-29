using DH.Domain.Entities;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;

namespace DH.Application.Games.Commands.Games;

public class CreateGameCopyCommand : IRequest
{
    public int Id { get; set; }
    public string webRootPath { get; set; } = string.Empty;
}

internal class CreateGameCopyCommandHandler : IRequestHandler<CreateGameCopyCommand>
{
    readonly IRepository<Game> repository;
    readonly IRepository<GameInventory> gameInvetoryRepository;

    public CreateGameCopyCommandHandler(
        IRepository<Game> repository,
        IRepository<GameInventory> gameInvetoryRepository)
    {
        this.repository = repository;
        this.gameInvetoryRepository = gameInvetoryRepository;
    }

    public async Task Handle(CreateGameCopyCommand request, CancellationToken cancellationToken)
    {
        var game = await this.repository.GetByAsyncWithTracking(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Game), request.Id);

        var gameCopy = await this.gameInvetoryRepository.GetByAsyncWithTracking(x => x.GameId == game.Id, cancellationToken)
             ?? throw new NotFoundException(nameof(GameInventory));

        gameCopy.TotalCopies++;
        gameCopy.AvailableCopies++;

        await this.repository.Update(game, cancellationToken);
    }
}