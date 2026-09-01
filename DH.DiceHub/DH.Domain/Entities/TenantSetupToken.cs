namespace DH.Domain.Entities;

public class TenantSetupToken
{
    public int Id { get; set; }
    public int TenantApplicationId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string TokenHash { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public DateTime? UsedAt { get; set; }

    public virtual TenantApplication TenantApplication { get; set; } = null!;
}
