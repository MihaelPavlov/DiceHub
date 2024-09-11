using DH.Domain.Adapters.QRManager;

namespace DH.Adapter.QRManager.QRCodeStates;

public class GameQRCodeState : IQRCodeState
{
    public Task HandleAsync(IQRCodeContext context, string data, CancellationToken cancellationToken)
    {

        return Task.CompletedTask;
    }
}
