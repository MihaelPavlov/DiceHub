using DH.Domain.Adapters.QRManager.StateModels;

namespace DH.Domain.Adapters.QRManager;

public interface IQRCodeManager
{
    Task<QrCodeValidationResult> ValidateQRCodeAsync(string data, CancellationToken cancellationToken);
    string CreateQRCode(QRReaderModel data, string webRootPath);
}
