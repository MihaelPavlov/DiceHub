using DH.Domain.Adapters.Localization;
using DH.Domain.Entities;
using DH.Domain.Models.RewardModels.Commands;
using DH.Domain.Services;
using DH.OperationResultCore.Exceptions;
using Mapster;
using MediatR;

namespace DH.Application.Rewards.Commands;

public record UpdateSystemRewardCommand(UpdateRewardDto Reward, string? FileName, string? ContentType, MemoryStream? ImageStream) : IRequest;

internal class UpdateSystemRewardCommandHandler : IRequestHandler<UpdateSystemRewardCommand>
{
    readonly IRewardService rewardService;
    readonly ILocalizationService localizer;

    public UpdateSystemRewardCommandHandler(
        IRewardService rewardService, ILocalizationService localizer)
    {
        this.rewardService = rewardService;
        this.localizer = localizer;
    }

    public async Task Handle(UpdateSystemRewardCommand request, CancellationToken cancellationToken)
    {
        if (!request.Reward.FieldsAreValid(out var validationErrors, localizer))
            throw new ValidationErrorsException(validationErrors);

        await this.rewardService.UpdateReward(
            request.Reward.Adapt<ChallengeReward>(),
            request.FileName,
            request.ContentType,
            request.ImageStream,
            cancellationToken
        );
    }
}