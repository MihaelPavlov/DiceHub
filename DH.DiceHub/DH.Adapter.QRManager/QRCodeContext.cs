using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.QRManager;
using DH.Domain.Adapters.QRManager.StateModels;
using DH.Domain.Entities;
using DH.Domain.Repositories;
using System.Text.Json;

namespace DH.Adapter.QRManager;

public class QRCodeContext : IQRCodeContext
{
    IQRCodeState _state;
    readonly IRepository<QrCodeScanAudit> scanAuditRepository;
    readonly IUserContext userContext;

    public QRCodeContext(IRepository<QrCodeScanAudit> scanAuditRepository, IUserContext userContext)
    {
        _state = null!;
        this.scanAuditRepository = scanAuditRepository;
        this.userContext = userContext;
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
            var jsonData = JsonSerializer.Serialize(data);
            var jsonErrorMessage = JsonSerializer.Serialize(errorMessage);

            await this.scanAuditRepository.AddAsync(new QrCodeScanAudit
            {
                TraceId = traceId,
                ScannedData = jsonData,
                ErrorMessage = jsonErrorMessage,
                ScannedAt = DateTime.UtcNow,
                UserId = this.userContext.UserId
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred while tracking QR code: {ex.Message}");
        }
    }
}
