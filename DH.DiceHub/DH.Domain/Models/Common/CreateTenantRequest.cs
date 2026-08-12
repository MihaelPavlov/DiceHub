namespace DH.Domain.Models.Common;

public class CreateTenantRequest
{
    public string TenantId { get; set; } = string.Empty;
    public string TenantName { get; set; } = string.Empty;
    public string Town { get; set; } = string.Empty;
    public string LogoFileName { get; set; } = string.Empty;
    public string OwnerEmail { get; set; } = string.Empty;
    public string ClubPhoneNumber { get; set; } = string.Empty;
    public string StarterProfile { get; set; } = "starter-pack";
}

public class CreateTenantResult
{
    public string TenantId { get; set; } = string.Empty;
    public string OwnerEmail { get; set; } = string.Empty;
    public string StarterProfile { get; set; } = string.Empty;
}
