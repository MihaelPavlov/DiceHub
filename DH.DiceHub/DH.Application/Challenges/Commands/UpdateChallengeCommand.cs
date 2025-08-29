using DH.Domain.Entities;
using DH.Domain.Models.ChallengeModels.Commands;
using DH.Domain.Repositories;
using Mapster;
using MediatR;

namespace DH.Application.Challenges.Commands;

public record UpdateChallengeCommand(UpdateChallengeDto Challenge) : IRequest;

internal class UpdateChallengeCommandHandler : IRequestHandler<UpdateChallengeCommand>
{
    readonly IRepository<Challenge> repository;
    public UpdateChallengeCommandHandler(IRepository<Challenge> repository)
    {
        this.repository = repository;
    }

    public async Task Handle(UpdateChallengeCommand request, CancellationToken cancellationToken)
    {
        await this.repository.Update(request.Challenge.Adapt<Challenge>(), cancellationToken);
    }
}
