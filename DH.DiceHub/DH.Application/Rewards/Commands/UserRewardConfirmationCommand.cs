﻿using DH.Domain.Entities;
using DH.Domain.Repositories;
using DH.Domain.Services.Publisher;
using DH.OperationResultCore.Exceptions;
using MediatR;

namespace DH.Application.Rewards.Commands;

public record UserRewardConfirmationCommand(int Id) : IRequest;

internal class UserRewardConfirmationCommandHandler(IEventPublisherService eventPublisherService, IRepository<UserChallengeReward> userChallengeRewardRepository) : IRequestHandler<UserRewardConfirmationCommand>
{
    readonly IEventPublisherService eventPublisherService = eventPublisherService;
    readonly IRepository<UserChallengeReward> userChallengeRewardRepository = userChallengeRewardRepository;

    // We assume based on previous validation that the user reward is valid and the person that is scanning the code is staff or admin.
    public async Task Handle(UserRewardConfirmationCommand request, CancellationToken cancellationToken)
    {
        var userReward = await this.userChallengeRewardRepository.GetByAsyncWithTracking(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(UserChallengeReward), request.Id);

        userReward.IsClaimed = true;

        await this.userChallengeRewardRepository.SaveChangesAsync(cancellationToken);

        await this.eventPublisherService.PublishRewardActionDetectedMessage(userReward.UserId, userReward.RewardId, false, userReward.IsClaimed);
    }
}
