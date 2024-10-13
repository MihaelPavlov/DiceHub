using DH.Domain.Adapters.QRManager.StateModels;

namespace DH.Domain.Adapters.QRManager;

/// <summary>
/// Interface defining the context for managing QR code states and operations.
/// </summary>
public interface IQRCodeContext
{
    /// <summary>
    /// Sets the current state for handling QR codes.
    /// </summary>
    /// <param name="state">The QR code state to set.</param>
    void SetState(IQRCodeState state);

    /// <summary>
    /// Handles QR code validation asynchronously using the current state.
    /// </summary>
    /// <param name="data">The QR code data to validate.</param>
    /// <param name="cancellationToken">The cancellation token to observe for cancellation requests.</param>
    /// <returns>A task representing the result of the validation process.</returns>
    Task<QrCodeValidationResult> HandleAsync(QRReaderModel data, CancellationToken cancellationToken);

    /// <summary>
    /// Tracks scanned QR code information asynchronously.
    /// </summary>
    /// <param name="traceId">The unique identifier for tracking.</param>
    /// <param name="data">The scanned QR code data.</param>
    /// <param name="errorMessage">An optional error message related to the scan.</param>
    /// <param name="cancellationToken">The cancellation token to observe for cancellation requests.</param>
    Task TrackScannedQrCode(string traceId, QRReaderModel data, Exception? errorMessage, CancellationToken cancellationToken);
}
