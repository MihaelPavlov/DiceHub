namespace DH.Domain.Entities;

public class QrCodeScanAudit
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string ScannedData { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public DateTime ScannedAt { get; set; }
}
