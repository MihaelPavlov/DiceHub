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

    public CreateChallengeCommandHandler(IChallengeService service)
    {
        this.service = service;
    }

    public async Task<int> Handle(CreateChallengeCommand request, CancellationToken cancellationToken)
    {
        if (!request.Challenge.FieldsAreValid(out var validationErrors))
            throw new ValidationErrorsException(validationErrors);

        var challengeId = await this.service.Create(request.Challenge.Adapt<Challenge>(), cancellationToken);

        return challengeId;
    }
}