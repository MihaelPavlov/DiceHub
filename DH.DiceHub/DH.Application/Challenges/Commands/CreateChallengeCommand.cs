
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
    readonly IRepository<Challenge> challengeRepository;
    readonly IRepository<ChallengeStatistic> challengeStatisticRepository;

    public CreateChallengeCommandHandler(IRepository<Challenge> challengeRepository, IRepository<ChallengeStatistic> challengeStatisticRepository)
    {
        this.challengeRepository = challengeRepository;
        this.challengeStatisticRepository = challengeStatisticRepository;
    }

    public async Task<int> Handle(CreateChallengeCommand request, CancellationToken cancellationToken)
    {
        if (!request.Challenge.FieldsAreValid(out var validationErrors))
            throw new ValidationErrorsException(validationErrors);

        var challenge = await this.challengeRepository.AddAsync(request.Challenge.Adapt<Challenge>(), cancellationToken) ??
            throw new BadRequestException("Challenge was not successfully created");

        await this.challengeStatisticRepository.AddAsync(new ChallengeStatistic
        {
            ChallengeId = challenge.Id,
            TotalCompletions = 0
        }, cancellationToken);

        return challenge.Id;
    }
}