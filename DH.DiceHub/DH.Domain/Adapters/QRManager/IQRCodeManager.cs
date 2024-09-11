namespace DH.Domain.Adapters.QRManager;

public interface IQRCodeManager
{
    Task ProcessQRCodeAsync(string data, CancellationToken cancellationToken);
    void CreateQRCode(string data, string webRootPath);
    void ValidateCode(string data);
}
