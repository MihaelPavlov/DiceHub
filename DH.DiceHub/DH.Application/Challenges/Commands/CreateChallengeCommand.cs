using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Models.ChallengeModels.Commands;
using DH.Domain.Repositories;
using Mapster;
using MediatR;

namespace DH.Application.Challenges.Commands;

public record CreateChallengeCommand(CreateChallengeDto Challenge) : IRequest<int>;

internal class CreateChallengeCommandHandler : IRequestHandler<CreateChallengeCommand, int>
{
    readonly IRepository<Challenge> repository;

    public CreateChallengeCommandHandler(IRepository<Challenge> repository)
    {
        this.repository = repository;
    }

    public async Task<int> Handle(CreateChallengeCommand request, CancellationToken cancellationToken)
    {
        if (!request.Challenge.FieldsAreValid(out var validationErrors))
            throw new ValidationErrorsException(validationErrors);

        var challenge = await this.repository.AddAsync(request.Challenge.Adapt<Challenge>(), cancellationToken);

        return challenge.Id;
    }
}