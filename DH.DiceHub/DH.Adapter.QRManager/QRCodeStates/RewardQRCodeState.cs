using DH.Domain.Adapters.QRManager.StateModels;
using DH.Domain.Adapters.QRManager;
using DH.Domain.Adapters.Authentication;
using DH.Domain.Repositories;
using DH.Domain.Entities;
using DH.Domain.Adapters.Authentication.Models.Enums;
using DH.OperationResultCore.Exceptions;

internal class RewardQRCodeState : IQRCodeState
{
    readonly IUserContext userContext;
    readonly IRepository<UserChallengeReward> userChallengeRewardRepository;

    public RewardQRCodeState(IUserContext userContext, IRepository<UserChallengeReward> userChallengeRewardRepository)
    {
        this.userContext = userContext;
        this.userChallengeRewardRepository = userChallengeRewardRepository;
    }

    public async Task<QrCodeValidationResult> HandleAsync(IQRCodeContext context, QRReaderModel data, CancellationToken cancellationToken)
    {
        var traceId = Guid.NewGuid().ToString();

        var result = new QrCodeValidationResult(data.Id, QrCodeType.Reward);

        if (this.userContext.RoleKey == (int)Role.User)
            return await SetError(context, data, result, "Access denied: only authorized staff can give rewards.", traceId, cancellationToken);

        if (!data.AdditionalData.TryGetValue("userId", out var userId))
            return await SetError(context, data, result, "Invalid request: User ID is missing.", traceId, cancellationToken);

        var userReward = await this.userChallengeRewardRepository
             .GetByAsync(x => x.Id == data.Id && x.UserId == userId, cancellationToken);

        if (userReward == null)
            return await SetError(context, data, result, "No reward found for the specified user.", traceId, cancellationToken);

        if (userReward.IsExpired)
            return await SetError(context, data, result, "This reward has expired and can no longer be claimed.", traceId, cancellationToken);

        if (userReward.IsClaimed)
            return await SetError(context, data, result, "This reward has already been claimed.", traceId, cancellationToken);

        await context.TrackScannedQrCode(traceId, data, null, cancellationToken);
        result.IsValid = true;
        return result;
    }

    private async Task<QrCodeValidationResult> SetError(IQRCodeContext context, QRReaderModel data, QrCodeValidationResult result, string errorMessage, string traceId, CancellationToken cancellationToken)
    {
        result.ErrorMessage = errorMessage;
        result.IsValid = false;
        await context.TrackScannedQrCode(traceId, data, new BadRequestException(errorMessage), cancellationToken);
        return result;
    }
}