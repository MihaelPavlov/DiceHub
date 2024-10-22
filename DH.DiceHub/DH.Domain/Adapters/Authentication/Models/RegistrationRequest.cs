namespace DH.Domain.Adapters.Authentication.Models;

public class RegistrationRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string DeviceToken { get; set; } = string.Empty;
}

public class RegistrationNotifcation
{
    public string Email { get; set; } = string.Empty;
}
