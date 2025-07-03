using DH.Domain.Entities;
using DH.Domain.Models.ChallengeModels.Commands;
using DH.Domain.Repositories;
using DH.Domain.Services;
using DH.OperationResultCore.Exceptions;
using MediatR;

namespace DH.Application.Challenges.Commands;

public record SaveCustomPeriodCommand(SaveCustomPeriodDto CustomPeriod) : IRequest;

internal class SaveCustomPeriodCommandHandler(
    IChallengeService challengeService) : IRequestHandler<SaveCustomPeriodCommand>
{
    readonly IChallengeService challengeService = challengeService;

    public async Task Handle(SaveCustomPeriodCommand request, CancellationToken cancellationToken)
    {
        if (!request.CustomPeriod.FieldsAreValid(out var validationErrors))
            throw new ValidationErrorsException(validationErrors);

        await this.challengeService.SaveCustomPeriod(request.CustomPeriod, cancellationToken);
    }
}
