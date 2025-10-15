using DH.Domain.Adapters.QRManager.StateModels;
using DH.Domain.Adapters.QRManager;
using DH.Domain.Adapters.Authentication;
using DH.Domain.Repositories;
using DH.Domain.Entities;
using DH.Domain.Adapters.Authentication.Models.Enums;
using DH.OperationResultCore.Exceptions;
using DH.Domain.Adapters.Localization;

internal class RewardQRCodeState : IQRCodeState
{
    readonly IUserContext userContext;
    readonly IRepository<UserChallengeReward> userChallengeRewardRepository;
    readonly ILocalizationService loc;

    public RewardQRCodeState(IUserContext userContext, IRepository<UserChallengeReward> userChallengeRewardRepository, ILocalizationService loc)
    {
        this.userContext = userContext;
        this.userChallengeRewardRepository = userChallengeRewardRepository;
        this.loc = loc;
    }

    public async Task<QrCodeValidationResult> HandleAsync(IQRCodeContext context, QRReaderModel data, CancellationToken cancellationToken)
    {
        var traceId = Guid.NewGuid().ToString();

        var result = new QrCodeValidationResult(data.Id, QrCodeType.Reward);

        if (this.userContext.RoleKey == (int)Role.User)
            return await SetError(context, data, result, this.loc["RewardQrCodeScannedAccessDenied"], traceId, cancellationToken);

        if (!data.AdditionalData.TryGetValue("userId", out var userId))
            return await SetError(context, data, result, this.loc["QrCodeScannedMissingUserId"], traceId, cancellationToken);

        var userReward = await this.userChallengeRewardRepository
             .GetByAsync(x => x.Id == data.Id && x.UserId == userId, cancellationToken);

        if (userReward == null)
            return await SetError(context, data, result, this.loc["RewardQrCodeScannedNoRewardFound"], traceId, cancellationToken);

        if (userReward.IsExpired)
            return await SetError(context, data, result, this.loc["RewardQrCodeScannedRewardExpired"], traceId, cancellationToken);

        if (userReward.IsClaimed)
            return await SetError(context, data, result, this.loc["RewardQrCodeScannedRewardAlreadyClaimed"], traceId, cancellationToken);

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