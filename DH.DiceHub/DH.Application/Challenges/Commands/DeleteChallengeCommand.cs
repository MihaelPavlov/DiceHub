using DH.Domain.Entities;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace DH.Application.Challenges.Commands;

public record DeleteChallengeCommand(int Id) : IRequest;

internal class DeleteChallengeCommandHandler : IRequestHandler<DeleteChallengeCommand>
{
    readonly IRepository<Challenge> repository;

    public DeleteChallengeCommandHandler(IRepository<Challenge> repository)
    {
        this.repository = repository;
    }

    public async Task Handle(DeleteChallengeCommand request, CancellationToken cancellationToken)
    {
        var challenge = await this.repository.GetByAsyncWithTracking(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Challenge), request.Id);

        if (challenge.UserChallenges.Count != 0)
            throw new ValidationException("Challenge has dependencies and cannot be deleted");

        await this.repository.Remove(challenge, cancellationToken);
    }
}
