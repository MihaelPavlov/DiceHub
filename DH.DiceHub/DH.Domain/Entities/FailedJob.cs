namespace DH.Domain.Entities;

public class FailedJob : TenantEntity
{
    public int Id { get; set; }
    public int Type { get; set; }
    public string Data { get; set; } = string.Empty;
    public DateTime FailedAt { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}
