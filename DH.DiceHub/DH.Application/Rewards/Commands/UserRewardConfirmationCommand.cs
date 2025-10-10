using DH.Domain.Services;
using MediatR;

namespace DH.Application.Rewards.Commands;

public record UserRewardConfirmationCommand(int Id) : IRequest;

internal class UserRewardConfirmationCommandHandler(
    IRewardService rewardService) : IRequestHandler<UserRewardConfirmationCommand>
{
    readonly IRewardService rewardService = rewardService;

    // We assume based on previous validation that the user reward is valid and the person that is scanning the code is staff or admin.
    public async Task Handle(UserRewardConfirmationCommand request, CancellationToken cancellationToken)
    {
        await this.rewardService.RewardConfirmation(request.Id, cancellationToken);
    }
}
