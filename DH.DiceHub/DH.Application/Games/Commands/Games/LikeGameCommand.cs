using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Data;
using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Games.Commands.Games;

public record LikeGameCommand(int Id) : IRequest;

internal class LikeGameCommandHanlder : IRequestHandler<LikeGameCommand>
{
    readonly ITenantDbContext dbContext;
    readonly IUserContext userContext;

    public LikeGameCommandHanlder(ITenantDbContext dbContext, IUserContext userContext)
    {
        this.dbContext = dbContext;
        this.userContext = userContext;
    }

    public async Task Handle(LikeGameCommand request, CancellationToken cancellationToken)
    {
        var gameRepository = dbContext.AcquireRepository<IRepository<Game>>();
        var gameLikesRepository = dbContext.AcquireRepository<IRepository<GameLike>>();

        var game = await gameRepository.GetByAsyncWithTracking(x => x.Id == request.Id, cancellationToken)
             ?? throw new NotFoundException(nameof(Game), request.Id);

        await gameLikesRepository.AddAsync(new GameLike
        {
            GameId = request.Id,
            UserId = userContext.UserId
        }, cancellationToken);
    }
}
