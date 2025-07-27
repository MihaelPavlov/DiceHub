namespace DH.Domain.Adapters.Authentication.Models;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
    public string? DeviceToken { get; set; }
    public string? TimeZone { get; set; }
}