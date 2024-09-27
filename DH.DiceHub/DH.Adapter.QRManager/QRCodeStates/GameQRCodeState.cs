using DH.Domain.Adapters.QRManager;

namespace DH.Adapter.QRManager.QRCodeStates;

public class GameQRCodeState : IQRCodeState
{
    public Task HandleAsync(IQRCodeContext context, string data, CancellationToken cancellationToken)
    {
        //TODO: Handle what happend if you scan a game QR code
        // Trigger the game challenge background job maybe
        return Task.CompletedTask;
    }
}
