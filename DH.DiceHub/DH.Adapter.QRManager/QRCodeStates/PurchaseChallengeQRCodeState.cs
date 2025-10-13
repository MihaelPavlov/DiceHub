using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Authentication.Models.Enums;
using DH.Domain.Adapters.QRManager;
using DH.Domain.Adapters.QRManager.StateModels;
using DH.Domain.Services;
using DH.OperationResultCore.Exceptions;

namespace DH.Adapter.QRManager.QRCodeStates;

public class PurchaseChallengeQRCodeState(
    IUserContext userContext,
    IUniversalChallengeProcessing universalChallengeProcessing) : IQRCodeState
{
    readonly IUserContext userContext = userContext;
    readonly IUniversalChallengeProcessing universalChallengeProcessing = universalChallengeProcessing;

    public async Task<QrCodeValidationResult> HandleAsync(IQRCodeContext context, QRReaderModel data, CancellationToken cancellationToken)
    {
        var traceId = Guid.NewGuid().ToString();

        var result = new QrCodeValidationResult(data.Id, QrCodeType.GameReservation);

        if (this.userContext.RoleKey == (int)Role.User)
            return await SetError(context, data, result, "Access denied: only authorized staff can scan reservations.", traceId, cancellationToken);

        if (!data.AdditionalData.TryGetValue("userId", out var userId))
            return await SetError(context, data, result, "Invalid request: User ID is missing.", traceId, cancellationToken);

        var resultFromQrCodeProcessing = await this.universalChallengeProcessing.PurchaseChallengeQrCodeProcessing(userId, cancellationToken);

        if (!resultFromQrCodeProcessing)
            return await SetError(context, data, result, "Processing was not successful.", traceId, cancellationToken);

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
