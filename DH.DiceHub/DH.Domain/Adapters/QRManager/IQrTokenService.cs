using DH.Domain.Adapters.QRManager.StateModels;

namespace DH.Domain.Adapters.QRManager;

/// <summary>
/// Issues and resolves the short opaque QR tokens (see <see cref="DH.Domain.Entities.QrToken"/>).
/// </summary>
public interface IQrTokenService
{
    /// <summary>
    /// Returns a token to encode in a QR for <paramref name="type"/> / <paramref name="entityId"/>.
    /// For per-user types (Reward, GameReservation, TableReservation, PurchaseChallenge) the
    /// user is taken from the current context and the token expires after a short window;
    /// for Game/Event a single long-lived token per entity is reused.
    /// </summary>
    Task<string> IssueAsync(QrCodeType type, int entityId, CancellationToken cancellationToken);

    /// <summary>
    /// Resolves a scanned token to a <see cref="QRReaderModel"/>, or null when the token is
    /// unknown, expired or already consumed.
    /// </summary>
    Task<QRReaderModel?> ResolveAsync(string token, CancellationToken cancellationToken);

    /// <summary>Marks a per-user token as used so it can't be replayed. No-op for static tokens.</summary>
    Task MarkConsumedAsync(string token, CancellationToken cancellationToken);
}
