namespace DH.Domain.Adapters.QRManager.StateModels;

public class QRReaderModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public QrCodeType Type { get; set; }
    public Dictionary<string, string> AdditionalData { get; set; } = [];
}
