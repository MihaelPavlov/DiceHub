namespace DH.Domain.Entities;

public abstract class TenantEntity : ITenantEntity
{
    public string TenantId { get; set; } = string.Empty;
}
