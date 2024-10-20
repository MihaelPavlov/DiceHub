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

    /// <summary>
    /// Generates a QR code image based on the provided data and saves it to the specified web root path.
    /// </summary>
    /// <param name="data">The data used to generate the QR code.</param>
    /// <param name="webRootPath">The root path where the QR code image will be saved.</param>
    /// <returns>Returns the file name of the generated QR code image.</returns>
    string CreateQRCode(QRReaderModel data, string webRootPath);
}
