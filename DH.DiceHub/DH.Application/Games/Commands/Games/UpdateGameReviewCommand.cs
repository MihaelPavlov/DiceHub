using DH.Domain.Adapters.Data;
using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Models.GameModels.Commands;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Games.Commands.Games;

public record UpdateGameReviewCommand(UpdateGameReviewDto UpdateGameReviewDto) : IRequest;

internal class UpdateGameReviewCommandHandler : IRequestHandler<UpdateGameReviewCommand>
{
    readonly ITenantDbContext dbContext;

    public UpdateGameReviewCommandHandler(ITenantDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task Handle(UpdateGameReviewCommand request, CancellationToken cancellationToken)
    {
        if (!request.UpdateGameReviewDto.FieldsAreValid(out var validationErrors))
            throw new ValidationErrorsException(validationErrors);

        var gameReviewRepository = dbContext.AcquireRepository<IRepository<GameReview>>();
        var gameReview = await gameReviewRepository.GetByAsync(x => x.Id == request.UpdateGameReviewDto.Id, cancellationToken);

        if (gameReview is null)
            throw new NotFoundException($"{nameof(GameReview)} with id {request.UpdateGameReviewDto.Id} was not found");

        gameReview.Review = request.UpdateGameReviewDto.Review;
        gameReview.UpdatedDate = DateTime.UtcNow;

        await gameReviewRepository.Update(gameReview, cancellationToken);
    }

}