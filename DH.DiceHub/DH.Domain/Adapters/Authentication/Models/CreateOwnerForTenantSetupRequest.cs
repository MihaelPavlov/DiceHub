namespace DH.Domain.Adapters.Authentication.Models;

public class CreateOwnerForTenantSetupRequest
{
    public string TenantId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ClubPhoneNumber { get; set; } = string.Empty;
}

public class CreateOwnerForTenantSetupResult
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string TemporaryPassword { get; set; } = string.Empty;
}
