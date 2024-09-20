using DH.Domain.Adapters.QRManager.StateModels;

namespace DH.Domain.Adapters.QRManager;

public interface IQRCodeManager
{
    Task ProcessQRCodeAsync(string data, CancellationToken cancellationToken);
    string CreateQRCode(QRReaderModel data, string webRootPath);
    void ValidateCode(string data);
}
