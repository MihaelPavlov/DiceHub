using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Data;
using DH.Domain.Entities;
using DH.Domain.Repositories;
using DH.Domain.Services;
using Mapster;

namespace DH.Application.Cqrs.GameCqrs;

public class CreateGameCommandHandler : AbstractCommandHandler<CreateGameCommand, int>, ICreateGameCommandHandler
{
    readonly ITenantDbContext _dbContext;
    readonly IIdentityDbContext applicationDbContext;
    readonly IGameService gameService;
    readonly IUserContext userContext;

    public CreateGameCommandHandler(ITenantDbContext dbContext, IGameService gameService, IIdentityDbContext applicationDbContext, IUserContext userContext)
    {
        this._dbContext = dbContext;
        this.applicationDbContext = applicationDbContext;
        this.gameService = gameService;
        this.userContext = userContext;
    }

    protected async override Task<int> HandleAsync(CreateGameCommand request, CancellationToken cancellationToken)
    {
      
        if (!request.Game.FieldsAreValied())
            throw new ArgumentException("Invalid game");

        var userId = this.userContext.UserId;
        var repo = _dbContext.AcquireRepository<IGameRepository>();
        var test = applicationDbContext.AcquireRepository<ITestRepository>();

        var res = await this.gameService.GetCompexDataAsync(cancellationToken);

        var gameId = await repo.CreateAsync(request.Game.Adapt<Game>(), cancellationToken);
        var testId = await test.CreateAsync(new Test { Name="TESTTTT"}, cancellationToken);

        return gameId;
    }
}
