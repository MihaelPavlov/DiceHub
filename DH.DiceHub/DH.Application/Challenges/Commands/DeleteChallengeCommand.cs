using DH.Domain.Services;
using MediatR;

namespace DH.Application.Challenges.Commands;

public record DeleteChallengeCommand(int Id) : IRequest;

internal class DeleteChallengeCommandHandler(IChallengeService challengeService) : IRequestHandler<DeleteChallengeCommand>
{
    readonly IChallengeService challengeService = challengeService;

    public async Task Handle(DeleteChallengeCommand request, CancellationToken cancellationToken) =>
        await this.challengeService.Delete(request.Id, cancellationToken);
}
