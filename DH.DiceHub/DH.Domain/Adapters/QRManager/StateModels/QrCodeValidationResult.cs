namespace DH.Domain.Adapters.QRManager.StateModels;

public class QrCodeValidationResult
{
    public QrCodeValidationResult(int objectId,QrCodeType type)
    {
        this.ObjectId = objectId;
        this.Type = type;
    }

    public int ObjectId { get; set; }
    public QrCodeType Type { get; set; }
    private bool isValid;

    public bool IsValid
    {
        get => string.IsNullOrEmpty(ErrorMessage) && isValid;
        set => isValid = value;
    }

    private string errorMessage = string.Empty;
    public string ErrorMessage
    {
        get => errorMessage;
        set
        {
            errorMessage = value;
            if (!string.IsNullOrEmpty(errorMessage))
            {
                IsValid = false;
            }
        }
    }
}
