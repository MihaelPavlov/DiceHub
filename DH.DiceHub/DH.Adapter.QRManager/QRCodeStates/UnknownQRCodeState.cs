using DH.Domain.Adapters.QRManager;
using DH.Domain.Adapters.QRManager.StateModels;

namespace DH.Adapter.QRManager.QRCodeStates;

internal class UnknownQRCodeState : IQRCodeState
{
    public async Task<QrCodeValidationResult> HandleAsync(IQRCodeContext context, QRReaderModel data, CancellationToken cancellationToken)
    {
        return new QrCodeValidationResult(QrCodeType.Unknown);
    }
}