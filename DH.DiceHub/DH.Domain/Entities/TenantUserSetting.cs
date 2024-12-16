namespace DH.Domain.Entities;

public class TenantUserSetting
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string AssistiveTouchSettings { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}
