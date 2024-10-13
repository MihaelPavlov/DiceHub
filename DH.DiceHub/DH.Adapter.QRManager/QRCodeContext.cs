using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.QRManager;
using DH.Domain.Adapters.QRManager.StateModels;
using DH.Domain.Entities;
using DH.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace DH.Adapter.QRManager;

public class QRCodeContext : IQRCodeContext
{
    IQRCodeState _state;
    readonly IServiceScopeFactory serviceScopeFactory;

    public QRCodeContext(IServiceScopeFactory serviceScopeFactory)
    {
        _state = null!;
        this.serviceScopeFactory = serviceScopeFactory;
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
            using (var scope = this.serviceScopeFactory.CreateScope())
            {
                var scanAuditRepository = scope.ServiceProvider.GetRequiredService<IRepository<QrCodeScanAudit>>();
                var userContext = scope.ServiceProvider.GetRequiredService<IUserContext>();

                var jsonData = JsonSerializer.Serialize(data);
                var jsonErrorMessage = JsonSerializer.Serialize(errorMessage);

                await scanAuditRepository.AddAsync(new QrCodeScanAudit
                {
                    TraceId = traceId,
                    ScannedData = jsonData,
                    ErrorMessage = jsonErrorMessage,
                    ScannedAt = DateTime.UtcNow,
                    UserId = userContext.UserId
                }, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred while tracking QR code: {ex.Message}");
        }
    }
}
