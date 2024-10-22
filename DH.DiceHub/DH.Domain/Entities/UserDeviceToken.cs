namespace DH.Domain.Entities;

public class UserDeviceToken
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string DeviceToken { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
