using DH.Domain.Adapters.Localization;
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
    readonly ILocalizationService localizer;

    public CreateSystemRewardCommandHandler(IRewardService rewardService, ILocalizationService localizer)
    {
        this.rewardService = rewardService;
        this.localizer = localizer;
    }

    public async Task<int> Handle(CreateSystemRewardCommand request, CancellationToken cancellationToken)
    {
        if (!request.Reward.FieldsAreValid(out var validationErrors, localizer))
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
