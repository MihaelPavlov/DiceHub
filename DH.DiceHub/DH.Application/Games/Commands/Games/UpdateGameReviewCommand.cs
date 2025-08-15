using DH.Domain.Adapters.Data;
using DH.Domain.Adapters.Localization;
using DH.Domain.Entities;
using DH.Domain.Models.GameModels.Commands;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;

namespace DH.Application.Games.Commands.Games;

public record UpdateGameReviewCommand(UpdateGameReviewDto UpdateGameReviewDto) : IRequest;

internal class UpdateGameReviewCommandHandler : IRequestHandler<UpdateGameReviewCommand>
{
    readonly ITenantDbContext dbContext;
    readonly ILocalizationService localizer;

    public UpdateGameReviewCommandHandler(ITenantDbContext dbContext, ILocalizationService localizer)
    {
        this.dbContext = dbContext;
        this.localizer = localizer;
    }

    public async Task Handle(UpdateGameReviewCommand request, CancellationToken cancellationToken)
    {
        if (!request.UpdateGameReviewDto.FieldsAreValid(out var validationErrors, localizer))
            throw new ValidationErrorsException(validationErrors);

        var gameReviewRepository = dbContext.AcquireRepository<IRepository<GameReview>>();
        var gameReview = await gameReviewRepository.GetByAsync(x => x.Id == request.UpdateGameReviewDto.Id, cancellationToken);

        if (gameReview is null)
            throw new NotFoundException(nameof(GameReview), request.UpdateGameReviewDto.Id);

        gameReview.Review = request.UpdateGameReviewDto.Review;
        gameReview.UpdatedDate = DateTime.UtcNow;

        await gameReviewRepository.Update(gameReview, cancellationToken);
    }

}