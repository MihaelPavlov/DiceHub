namespace DH.Domain.Models.Common;

public class GetTenantListQueryModel
{
    public string Id { get; set; } = string.Empty;
    public string TenantName { get; set; } = string.Empty;
    public string LogoFileName { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PublicWebsite { get; set; } = string.Empty;
    public string SocialPage { get; set; } = string.Empty;
    public string DiscordServer { get; set; } = string.Empty;
    public int TenantStatus { get; set; }
    public DateTime CreatedDate { get; set; }
    public int AverageMaxCapacity { get; set; }
    public string StartWorkingHours { get; set; } = string.Empty;
    public string EndWorkingHours { get; set; } = string.Empty;
    public string DaysOff { get; set; } = string.Empty;
    public string ClubPhoneNumber { get; set; } = string.Empty;
}
