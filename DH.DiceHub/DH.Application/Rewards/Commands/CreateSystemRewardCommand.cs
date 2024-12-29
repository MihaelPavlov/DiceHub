using DH.Domain.Entities;
using DH.Domain.Models.RewardModels.Commands;
using DH.Domain.Services;
using DH.OperationResultCore.Exceptions;
using Mapster;
using MediatR;

namespace DH.Application.Rewards.Commands;

public record CreateSystemRewardCommand(CreateRewardDto Reward, string FileName, string ContentType, MemoryStream ImageStream) : IRequest<int>;

internal class CreateSystemRewardCommandHandler : IRequestHandler<CreateSystemRewardCommand, int>
{
    readonly IRewardService rewardService;

    public CreateSystemRewardCommandHandler(IRewardService rewardService)
    {
        this.rewardService = rewardService;
    }

    public async Task<int> Handle(CreateSystemRewardCommand request, CancellationToken cancellationToken)
    {
        if (!request.Reward.FieldsAreValid(out var validationErrors))
            throw new ValidationErrorsException(validationErrors);

        return await this.rewardService.CreateReward(
            request.Reward.Adapt<ChallengeReward>(),
            request.FileName,
            request.ContentType,
            request.ImageStream,
            cancellationToken
        );
    }
}
