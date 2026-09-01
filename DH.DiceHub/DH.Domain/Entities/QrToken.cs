using DH.Domain.Adapters.QRManager;

namespace DH.Domain.Entities;

/// <summary>
/// An opaque short code shown as a QR instead of an encrypted payload, so the
/// code stays tiny (~QR version 1) and scans reliably on poor cameras.
/// The scanner sends the token back and the server resolves it to the real
/// entity + user. Legacy encrypted QR blobs are still accepted as a fallback.
/// </summary>
public class QrToken : TenantEntity
{
    public int Id { get; set; }

    /// <summary>The value encoded in the QR: 1 type digit + 11 uppercase base32 chars.</summary>
    public string Token { get; set; } = string.Empty;

    public QrCodeType Type { get; set; }

    /// <summary>Game / event / reservation / reward id, depending on <see cref="Type"/>.</summary>
    public int EntityId { get; set; }

    /// <summary>The user the code belongs to. Null for the static Game/Event codes.</summary>
    public string? UserId { get; set; }

    public DateTime CreatedDate { get; set; }

    /// <summary>Null = never expires (static Game/Event). Set for per-user codes.</summary>
    public DateTime? ExpiresDate { get; set; }

    /// <summary>Set on the first successful scan of a per-user code (single use).</summary>
    public DateTime? ConsumedDate { get; set; }
}
