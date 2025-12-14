using DH.Domain.Enums;

namespace DH.Domain.Entities;

public class Tenant
{
    public string Id { get; set; } = string.Empty;
    public string TenantName { get; set; } = string.Empty;
    public string Town { get; set; } = string.Empty;
    public TenantStatus TenantStatus { get; set; }
    public DateTime CreatedDate { get; set; }

    // Optional for now. Under discsussion
    public string RegisterQrCode { get; set; } = string.Empty;

    public int TenantSettingId { get; set; }
    public virtual TenantSetting TenantSetting { get; set; } = null!;
}
