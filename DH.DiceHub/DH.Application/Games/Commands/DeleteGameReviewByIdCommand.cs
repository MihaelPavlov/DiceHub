using DH.Domain.Adapters.Data;
using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Games.Commands;

public record DeleteGameReviewByIdCommand(int Id) : IRequest;

internal class DeleteGameReviewByIdCommandHandler : IRequestHandler<DeleteGameReviewByIdCommand>
{
    readonly ITenantDbContext dbContext;

    public DeleteGameReviewByIdCommandHandler(ITenantDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task Handle(DeleteGameReviewByIdCommand request, CancellationToken cancellationToken)
    {
        var gameReviewRepository = dbContext.AcquireRepository<IRepository<GameReview>>();
        var gameReview = await gameReviewRepository.GetByAsync(x => x.Id == request.Id, cancellationToken);

        if (gameReview is null)
            throw new NotFoundException($"{nameof(GameReview)} with id {request.Id} was not found");

        await gameReviewRepository.Remove(gameReview, cancellationToken);
    }
}
