using DH.Domain.Adapters.Statistics;
using DH.Domain.Adapters.Statistics.Services;
using DH.Domain.Entities;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;

namespace DH.Application.Rewards.Commands;

public record UserRewardConfirmationCommand(int Id) : IRequest;

internal class UserRewardConfirmationCommandHandler(
    IStatisticQueuePublisher statisticQueuePublisher,
    IRepository<UserChallengeReward> userChallengeRewardRepository) : IRequestHandler<UserRewardConfirmationCommand>
{
    readonly IStatisticQueuePublisher statisticQueuePublisher = statisticQueuePublisher;
    readonly IRepository<UserChallengeReward> userChallengeRewardRepository = userChallengeRewardRepository;

    // We assume based on previous validation that the user reward is valid and the person that is scanning the code is staff or admin.
    public async Task Handle(UserRewardConfirmationCommand request, CancellationToken cancellationToken)
    {
        var userReward = await this.userChallengeRewardRepository.GetByAsyncWithTracking(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(UserChallengeReward), request.Id);

        userReward.IsClaimed = true;

        await this.userChallengeRewardRepository.SaveChangesAsync(cancellationToken);

        await this.statisticQueuePublisher.PublishAsync(new StatisticJobQueue.RewardActionDetectedJob(
            userReward.UserId, userReward.RewardId,
            CollectedDate: DateTime.UtcNow, ExpiredDate: null,
            IsExpired: false, IsCollected: userReward.IsClaimed));
    }
}
