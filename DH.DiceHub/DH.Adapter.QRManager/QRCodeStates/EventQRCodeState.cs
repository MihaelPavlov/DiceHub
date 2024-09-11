using DH.Domain.Adapters.QRManager;

namespace DH.Adapter.QRManager.QRCodeStates;

internal class EventQRCodeState : IQRCodeState
{
    public Task HandleAsync(IQRCodeContext context, string data, CancellationToken cancellationToken)
    {

        return Task.CompletedTask;
    }
}
