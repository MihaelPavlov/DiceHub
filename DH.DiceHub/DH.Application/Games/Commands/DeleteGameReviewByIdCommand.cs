using DH.Application.Cqrs;
using DH.Domain.Adapters.Data;
using DH.Domain.Cqrs;
using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Repositories;

namespace DH.Application.Games.Commands;

public record DeleteGameReviewByIdCommand(int Id) : ICommand<bool>;

internal class DeleteGameReviewByIdCommandHandler : AbstractCommandHandler<DeleteGameReviewByIdCommand, bool>
{
    readonly ITenantDbContext dbContext;

    public DeleteGameReviewByIdCommandHandler(ITenantDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    protected override async Task<bool> HandleAsync(DeleteGameReviewByIdCommand request, CancellationToken cancellationToken)
    {
        var gameReviewRepository = dbContext.AcquireRepository<IRepository<GameReview>>();
        var gameReview = await gameReviewRepository.GetByAsync(x => x.Id == request.Id, cancellationToken);

        if (gameReview is null)
            throw new NotFoundException($"{nameof(GameReview)} with id {request.Id} was not found");

        await gameReviewRepository.Remove(gameReview, cancellationToken);

        return true;
    }
}
