using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Data;
using DH.Domain.Entities;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;

namespace DH.Application.Games.Commands.Games;

public record DislikeGameCommand(int Id) : IRequest;

internal class DislikeGameCommandHandler : IRequestHandler<DislikeGameCommand>
{
    readonly ITenantDbContext dbContext;
    readonly IUserContext userContext;

    public DislikeGameCommandHandler(ITenantDbContext dbContext, IUserContext userContext)
    {
        this.dbContext = dbContext;
        this.userContext = userContext;
    }

    public async Task Handle(DislikeGameCommand request, CancellationToken cancellationToken)
    {
        var gameRepository = dbContext.AcquireRepository<IRepository<Game>>();
        var gameLikesRepository = dbContext.AcquireRepository<IRepository<GameLike>>();

        var game = await gameRepository.GetByAsyncWithTracking(x => x.Id == request.Id, cancellationToken)
             ?? throw new NotFoundException(nameof(Game), request.Id);

        var like = await gameLikesRepository.GetByAsync(x => x.UserId == userContext.UserId && x.GameId == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(GameLike));

        await gameLikesRepository.Remove(like, cancellationToken);
    }
}
