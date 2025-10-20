using DH.Domain.Adapters.Localization;
using DH.Domain.Entities;
using DH.Domain.Models.ChallengeModels.Commands;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;

namespace DH.Application.Challenges.Commands;

public record UpdateUniversalChallengeCommand(UpdateUniversalChallengeDto UniversalChallenge) : IRequest;

internal class UpdateUniversalChallengeCommandHandler(
    IRepository<UniversalChallenge> repository,
    ILocalizationService localizer) : IRequestHandler<UpdateUniversalChallengeCommand>
{
    readonly IRepository<UniversalChallenge> repository = repository;
    readonly ILocalizationService localizer = localizer;

    public async Task Handle(UpdateUniversalChallengeCommand request, CancellationToken cancellationToken)
    {
        if (!request.UniversalChallenge.FieldsAreValid(out var validationErrors, localizer))
            throw new ValidationErrorsException(validationErrors);

        var dbUniversalChallenge = await this.repository.GetByAsyncWithTracking(x => x.Id == request.UniversalChallenge.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(UniversalChallenge), request.UniversalChallenge.Id);

        if (dbUniversalChallenge.Attempts != request.UniversalChallenge.Attempts)
            dbUniversalChallenge.Attempts = request.UniversalChallenge.Attempts;

        if (dbUniversalChallenge.MinValue != request.UniversalChallenge.MinValue)
            dbUniversalChallenge.MinValue = request.UniversalChallenge.MinValue;

        if (dbUniversalChallenge.RewardPoints != request.UniversalChallenge.RewardPoints)
            dbUniversalChallenge.RewardPoints = request.UniversalChallenge.RewardPoints;

        await this.repository.SaveChangesAsync(cancellationToken);
    }
}