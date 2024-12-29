using DH.Domain.Entities;
using DH.Domain.Models.RewardModels.Commands;
using DH.Domain.Repositories;
using DH.Domain.Services;
using DH.OperationResultCore.Exceptions;
using Mapster;
using MediatR;

namespace DH.Application.Rewards.Commands;

public record UpdateSystemRewardCommand(UpdateRewardDto Reward, string? FileName, string? ContentType, MemoryStream? ImageStream) : IRequest;

internal class UpdateSystemRewardCommandHandler : IRequestHandler<UpdateSystemRewardCommand>
{
    readonly IRewardService rewardService;
    readonly IRepository<ChallengeReward> repository;

    public UpdateSystemRewardCommandHandler(IRewardService rewardService, IRepository<ChallengeReward> repository)
    {
        this.rewardService = rewardService;
        this.repository = repository;
    }

    public async Task Handle(UpdateSystemRewardCommand request, CancellationToken cancellationToken)
    {
        if (!request.Reward.FieldsAreValid(out var validationErrors))
            throw new ValidationErrorsException(validationErrors);

        if (request.Reward.ImageId == null &&
            request.FileName != null &&
            request.ContentType != null &&
            request.ImageStream != null)
        {
            await this.rewardService.UpdateReward(
                request.Reward.Adapt<ChallengeReward>(),
                request.FileName,
                request.ContentType,
                request.ImageStream,
                cancellationToken
            );

            return;
        }

        await this.repository.Update(request.Reward.Adapt<ChallengeReward>(), cancellationToken);
    }
}