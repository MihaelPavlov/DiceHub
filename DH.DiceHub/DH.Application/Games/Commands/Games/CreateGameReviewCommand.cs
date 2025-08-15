using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Data;
using DH.Domain.Adapters.Localization;
using DH.Domain.Entities;
using DH.Domain.Models.GameModels.Commands;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using Mapster;
using MediatR;

namespace DH.Application.Games.Commands.Games;

public record CreateGameReviewCommand(CreateGameReviewDto GameReview) : IRequest<int>;

internal class CreateGameReviewCommandHandler : IRequestHandler<CreateGameReviewCommand, int>
{
    readonly ITenantDbContext dbContext;
    readonly IUserContext userContext;
    readonly ILocalizationService localizer;

    public CreateGameReviewCommandHandler(ITenantDbContext dbContext, IUserContext userContext, ILocalizationService localizer)
    {
        this.dbContext = dbContext;
        this.userContext = userContext;
        this.localizer = localizer;
    }

    public async Task<int> Handle(CreateGameReviewCommand request, CancellationToken cancellationToken)
    {
        if (!request.GameReview.FieldsAreValid(out var validationErrors, localizer))
            throw new ValidationErrorsException(validationErrors);

        var gameReviewRepository = dbContext.AcquireRepository<IRepository<GameReview>>();
        var createModel = request.GameReview.Adapt<GameReview>();
        createModel.UserId = userContext.UserId;
        createModel.CreatedDate = DateTime.UtcNow;
        createModel.UpdatedDate = DateTime.UtcNow;

        var gameReview = await gameReviewRepository.AddAsync(createModel, cancellationToken);

        return gameReview.Id;
    }
}