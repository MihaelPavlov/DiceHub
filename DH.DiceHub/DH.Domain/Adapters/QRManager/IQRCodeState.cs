namespace DH.Domain.Adapters.QRManager;

public interface IQRCodeState
{
    Task HandleAsync(IQRCodeContext context, string data, CancellationToken cancellationToken);
}
