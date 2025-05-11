namespace DH.Domain.Adapters.Authentication.Models;

public class UserRegistrationResponse
{
    public string? UserId { get; set; }
    public bool IsRegistrationSuccessfully => !string.IsNullOrWhiteSpace(UserId);
    public bool IsEmailConfirmationSendedSuccessfully { get; set; }
}
