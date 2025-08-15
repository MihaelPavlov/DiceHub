using DH.Domain.Adapters.Localization;
using DH.Domain.Entities;
using DH.Domain.Models.ChallengeModels.Commands;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using Mapster;
using MediatR;

namespace DH.Application.Challenges.Commands;

public record UpdateChallengeCommand(UpdateChallengeDto Challenge) : IRequest;

internal class UpdateChallengeCommandHandler : IRequestHandler<UpdateChallengeCommand>
{
    readonly IRepository<Challenge> repository;
    readonly ILocalizationService localizer;
    public UpdateChallengeCommandHandler(IRepository<Challenge> repository, ILocalizationService localizer)
    {
        this.repository = repository;
        this.localizer = localizer;
    }

    public async Task Handle(UpdateChallengeCommand request, CancellationToken cancellationToken)
    {
        if (!request.Challenge.FieldsAreValid(out var validationErrors, localizer))
            throw new ValidationErrorsException(validationErrors);

        await this.repository.Update(request.Challenge.Adapt<Challenge>(), cancellationToken);
    }
}
