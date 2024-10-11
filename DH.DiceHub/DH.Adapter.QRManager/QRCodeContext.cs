using DH.Domain.Adapters.QRManager;
using DH.Domain.Adapters.QRManager.StateModels;
using DH.Domain.Entities;
using DH.Domain.Repositories;

namespace DH.Adapter.QRManager;

public class QRCodeContext : IQRCodeContext
{
    private IQRCodeState _state;
    private readonly IRepository<QrCodeScanAudit> scanAuditRepository;

    public QRCodeContext(IRepository<QrCodeScanAudit> scanAuditRepository)
    {
        _state = null!;
        this.scanAuditRepository = scanAuditRepository;
    }

    public void SetState(IQRCodeState state)
    {
        _state = state;
    }

    public async Task<QrCodeValidationResult> HandleAsync(QRReaderModel data, CancellationToken cancellationToken)
    {
        return await _state.HandleAsync(this, data, cancellationToken);
    }

    public async Task TrackScannedQrCode(string traceId, QRReaderModel data, Exception? errorMessage, CancellationToken cancellationToken)
    {
        try
        {

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred while tracking QR code: {ex.Message}");
        }
    }
}
