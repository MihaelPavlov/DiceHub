using DH.Domain.Adapters.QRManager;
using DH.Domain.Adapters.QRManager.StateModels;

namespace DH.Adapter.QRManager.QRCodeStates;

internal class EventQRCodeState : IQRCodeState
{
    public Task<QrCodeValidationResult> HandleAsync(IQRCodeContext context, QRReaderModel data, CancellationToken cancellationToken)
    {
        return Task.FromResult(new QrCodeValidationResult(data.Id, QrCodeType.Event));
    }
}
