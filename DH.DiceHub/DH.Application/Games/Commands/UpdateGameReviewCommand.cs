using DH.Application.Cqrs;
using DH.Domain.Adapters.Data;
using DH.Domain.Cqrs;
using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Models.GameModels.Commands;
using DH.Domain.Repositories;

namespace DH.Application.Games.Commands;

public record UpdateGameReviewCommand(UpdateGameReviewDto UpdateGameReviewDto) : ICommand<bool>;

internal class UpdateGameReviewCommandHandler : AbstractCommandHandler<UpdateGameReviewCommand, bool>
{
    readonly ITenantDbContext dbContext;

    public UpdateGameReviewCommandHandler(ITenantDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    protected override async Task<bool> HandleAsync(UpdateGameReviewCommand request, CancellationToken cancellationToken)
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

        return true;
    }
}