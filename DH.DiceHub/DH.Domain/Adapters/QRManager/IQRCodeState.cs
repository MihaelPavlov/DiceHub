using DH.Domain.Adapters.QRManager.StateModels;

namespace DH.Domain.Adapters.QRManager;

public interface IQRCodeState
{
    Task<QrCodeValidationResult> HandleAsync(IQRCodeContext context, QRReaderModel data, CancellationToken cancellationToken);
}
