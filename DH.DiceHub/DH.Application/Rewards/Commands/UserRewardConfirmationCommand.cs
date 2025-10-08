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
        //TODO: Move the logic inside the RewardService
        // WE need also to get the TenantSetting and check if its customPeriod setting ON 
        // If its ON 
        // Then Getting ActiveCustomPeriod for user + the CustomerUserUniversalChallenges and find the UserXRewards Challenge
        // We should add the userAttemps+=1 and check if the Challenge is completed
        // If its completed we need to indicate that the challenge is completed and return to this handler and here to call challengeHubClient.SendUniversalChallengeCompleted()
        // iF ITS updated we need to indicate the the challenge is updated and callchallengeHubClient.SendUniversalChallengeUpdated()
        // And the same if Custom Period is OFF
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
