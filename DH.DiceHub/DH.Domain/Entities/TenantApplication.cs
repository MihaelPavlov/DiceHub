using DH.Domain.Enums;

namespace DH.Domain.Entities;

public class TenantApplication
{
    public int Id { get; set; }
    public string ApplicantType { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsEmailVerified { get; set; }
    public bool IsPhoneVerified { get; set; }
    public string Address { get; set; } = string.Empty;
    public string PublicWebsite { get; set; } = string.Empty;
    public string SocialPage { get; set; } = string.Empty;
    public string DiscordServer { get; set; } = string.Empty;
    public string PhotoUrl { get; set; } = string.Empty;
    public TenantApplicationStatus Status { get; set; } = TenantApplicationStatus.PendingVerification;
    public DateTime CreatedDate { get; set; }
    public DateTime? ReviewedDate { get; set; }
    public string? ReviewedByUserId { get; set; }
    public string? ReviewNote { get; set; }
}
