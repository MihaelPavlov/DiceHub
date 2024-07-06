using DH.Application.Cqrs;
using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Data;
using DH.Domain.Cqrs;
using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Models.GameModels.Commands;
using DH.Domain.Repositories;
using Mapster;

namespace DH.Application.Games.Commands;

public record CreateGameReviewCommand(CreateGameReviewDto GameReview) : ICommand<int>;

internal class CreateGameReviewCommandHandler : AbstractCommandHandler<CreateGameReviewCommand, int>
{
    readonly ITenantDbContext dbContext;
    readonly IUserContext userContext;

    public CreateGameReviewCommandHandler(ITenantDbContext dbContext, IUserContext userContext)
    {
        this.dbContext = dbContext;
        this.userContext = userContext;
    }

    protected override async Task<int> HandleAsync(CreateGameReviewCommand request, CancellationToken cancellationToken)
    {
        if (!request.GameReview.FieldsAreValid(out var validationErrors))
            throw new ValidationErrorsException(validationErrors);

        var gameReviewRepository = dbContext.AcquireRepository<IRepository<GameReview>>();
        var createModel = request.GameReview.Adapt<GameReview>();
        createModel.UserId = userContext.UserId;

        var gameReview = await gameReviewRepository.AddAsync(request.GameReview.Adapt<GameReview>(), cancellationToken);

        return gameReview.Id;
    }
}