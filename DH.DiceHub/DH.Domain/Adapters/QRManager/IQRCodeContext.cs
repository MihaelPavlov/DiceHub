namespace DH.Domain.Adapters.QRManager;

public interface IQRCodeContext
{
    void SetState(IQRCodeState state);
    Task HandleAsync(string data, CancellationToken cancellationToken);
}
