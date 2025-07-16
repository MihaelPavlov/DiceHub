namespace DH.Domain.Adapters.Authentication.Models;

public class EmployeeResult
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsEmailChanged { get; set; }
}
