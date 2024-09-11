using DH.Domain.Adapters.QRManager;

namespace DH.Adapter.QRManager;

public class QRCodeContext : IQRCodeContext
{
    private IQRCodeState _state;

    public QRCodeContext()
    {
        _state = null;
    }

    public void SetState(IQRCodeState state)
    {
        _state = state;
    }

    public async Task HandleAsync(string data, CancellationToken cancellationToken)
    {
        await _state.HandleAsync(this, data, cancellationToken);
    }
}
