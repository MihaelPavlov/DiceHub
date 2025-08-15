using DH.Domain.Adapters.Localization;
using DH.Domain.Entities;
using DH.Domain.Models.ChallengeModels.Commands;
using DH.Domain.Services;
using DH.OperationResultCore.Exceptions;
using Mapster;
using MediatR;

namespace DH.Application.Challenges.Commands;

public record CreateChallengeCommand(CreateChallengeDto Challenge) : IRequest<int>;

internal class CreateChallengeCommandHandler : IRequestHandler<CreateChallengeCommand, int>
{
    readonly IChallengeService service;
    readonly ILocalizationService localizer;

    public CreateChallengeCommandHandler(IChallengeService service, ILocalizationService localizer)
    {
        this.service = service;
        this.localizer = localizer;
    }

    public async Task<int> Handle(CreateChallengeCommand request, CancellationToken cancellationToken)
    {
        if (!request.Challenge.FieldsAreValid(out var validationErrors, localizer))
            throw new ValidationErrorsException(validationErrors);

        var challengeId = await this.service.Create(request.Challenge.Adapt<Challenge>(), cancellationToken);

        return challengeId;
    }
}