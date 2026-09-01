using DH.Domain.Adapters.QRManager;

namespace DH.Domain.Models.ScannerModels.Queries;

public class QrIssueRequest
{
    public QrCodeType Type { get; set; }
    public int EntityId { get; set; }
}

public class QrIssueResponse
{
    public string Token { get; set; } = string.Empty;
}
