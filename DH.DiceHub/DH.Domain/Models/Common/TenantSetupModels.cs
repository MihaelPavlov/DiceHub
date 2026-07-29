namespace DH.Domain.Models.Common;

public class CompleteTenantSetupRequest
{
    public string Token { get; set; } = string.Empty;
    public string ClubName { get; set; } = string.Empty;
    public int AverageMaxCapacity { get; set; }
    public string StartWorkingHours { get; set; } = string.Empty;
    public string EndWorkingHours { get; set; } = string.Empty;
    public string ClubPhoneNumber { get; set; } = string.Empty;
    public List<string> DaysOff { get; set; } = [];
    public List<int> SelectedGameIds { get; set; } = [];
}

public class CompleteTenantSetupResult
{
    public string TenantId { get; set; } = string.Empty;
    public string TenantName { get; set; } = string.Empty;
    public string OwnerEmail { get; set; } = string.Empty;
}
