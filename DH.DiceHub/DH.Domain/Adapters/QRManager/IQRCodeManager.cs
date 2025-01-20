using DH.Domain.Adapters.QRManager.StateModels;

namespace DH.Domain.Adapters.QRManager;

/// <summary>
/// Provides methods to validate and generate QR codes.
/// </summary>
public interface IQRCodeManager
{
    /// <summary>
    /// Validates the given QR code data asynchronously.
    /// </summary>
    /// <param name="data">The QR code data to be validated.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>Returns a <see cref="QrCodeValidationResult"/> containing the result of the validation.</returns>
    Task<QrCodeValidationResult> ValidateQRCodeAsync(string data, CancellationToken cancellationToken);
}
