using DH.Domain.Adapters.Data;
using DH.Domain.Entities;
using DH.Domain.Repositories;
using DH.Domain.Services;
using Mapster;

namespace DH.Application.Cqrs.GameCqrs;

public class CreateGameCommandHandler : AbstractCommandHandler<CreateGameCommand, int>, ICreateGameCommandHandler
{
    readonly IDBContext _dbContext;
    readonly IGameService gameService;

    public CreateGameCommandHandler(IDBContext dbContext, IGameService gameService)
    {
        _dbContext = dbContext;
        this.gameService = gameService;
    }

    protected async override Task<int> HandleAsync(CreateGameCommand request, CancellationToken cancellationToken)
    {
        if (!request.Game.FieldsAreValied())
            throw new ArgumentException("Invalid game");

        var repo = _dbContext.AcquireRepository<IGameRepository>();

        var res = await this.gameService.GetCompexDataAsync(cancellationToken);

        var gameId = await repo.CreateAsync(request.Game.Adapt<Game>(), cancellationToken);

        return gameId;
    }
}
