using DH.Domain.Adapters.Localization;
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
    readonly ILocalizationService localizer;

    public UpdateSystemRewardCommandHandler(IRewardService rewardService, IRepository<ChallengeReward> repository, ILocalizationService localizer)
    {
        this.rewardService = rewardService;
        this.repository = repository;
        this.localizer = localizer;
    }

    public async Task Handle(UpdateSystemRewardCommand request, CancellationToken cancellationToken)
    {
        if (!request.Reward.FieldsAreValid(out var validationErrors, localizer))
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