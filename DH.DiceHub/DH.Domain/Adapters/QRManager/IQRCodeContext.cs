using DH.Domain.Adapters.QRManager.StateModels;

namespace DH.Domain.Adapters.QRManager;

public interface IQRCodeContext
{
    void SetState(IQRCodeState state);
    Task<QrCodeValidationResult> HandleAsync(QRReaderModel data, CancellationToken cancellationToken);
    Task TrackScannedQrCode(string traceId, QRReaderModel data, Exception? errorMessage, CancellationToken cancellationToken);
}
