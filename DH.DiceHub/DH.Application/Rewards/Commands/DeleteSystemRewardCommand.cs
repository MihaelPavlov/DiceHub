using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Rewards.Commands;

public record DeleteSystemRewardCommand(int Id) : IRequest;

internal class DeleteSystemRewardCommandHandler : IRequestHandler<DeleteSystemRewardCommand>
{
    readonly IRepository<ChallengeReward> repository;

    public DeleteSystemRewardCommandHandler(IRepository<ChallengeReward> repository)
    {
        this.repository = repository;
    }

    public async Task Handle(DeleteSystemRewardCommand request, CancellationToken cancellationToken)
    {
        var challengeReward = await this.repository.GetByAsyncWithTracking(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(ChallengeReward), request.Id);

        challengeReward.IsDeleted = true;

        await this.repository.Update(challengeReward, cancellationToken);
    }
}