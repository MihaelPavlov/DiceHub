using DH.Domain.Adapters.QRManager;

namespace DH.Adapter.QRManager.QRCodeStates;

internal class UnknownQRCodeState : IQRCodeState
{
    public Task HandleAsync(IQRCodeContext context, string data, CancellationToken cancellationToken)
    {

        return Task.CompletedTask;
    }
}