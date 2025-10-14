using DH.Domain.Adapters.Localization;
using DH.Domain.Adapters.QRManager;
using DH.Domain.Adapters.QRManager.StateModels;
using DH.Domain.Entities;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;

namespace DH.Adapter.QRManager.QRCodeStates;

public class GameQRCodeState : IQRCodeState
{
    readonly IRepository<Game> gameRepository;
    readonly ILocalizationService loc;

    public GameQRCodeState(IRepository<Game> gameRepository, ILocalizationService loc)
    {
        this.gameRepository = gameRepository;
        this.loc = loc;
    }

    public async Task<QrCodeValidationResult> HandleAsync(IQRCodeContext context, QRReaderModel data, CancellationToken cancellationToken)
    {
        var traceId = Guid.NewGuid().ToString();
        var validationResult = new QrCodeValidationResult(data.Id, QrCodeType.Game);
        try
        {
            var game = await this.gameRepository.GetByAsyncWithTracking(x => x.Id == data.Id, cancellationToken);

            if (game == null)
            {
                validationResult.ErrorMessage = this.loc["GameQrCodeScannedIsInvalid"];
                validationResult.IsValid = false;
                await context.TrackScannedQrCode(traceId, data, new NotFoundException(validationResult.ErrorMessage), cancellationToken);
                return validationResult;
            }

            await context.TrackScannedQrCode(traceId, data, null, cancellationToken);
            validationResult.IsValid = true;
        }
        catch (Exception ex)
        {
            validationResult.IsValid = false;
            validationResult.ErrorMessage = this.loc["QrCodeScannedProcessingFailed"];
            await context.TrackScannedQrCode(traceId, data, ex, cancellationToken);
        }

        return validationResult;
    }
}
