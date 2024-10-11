namespace DH.Domain.Adapters.QRManager.StateModels;

public class QrCodeValidationResult
{
    public QrCodeValidationResult(QrCodeType type)
    {
        this.Type = type;
    }

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
