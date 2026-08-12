using DH.Domain.Enums;

namespace DH.Domain.Models.Common;

public class GetTenantListQueryModel
{
    public string Id { get; set; } = string.Empty;
    public string TenantName { get; set; } = string.Empty;
    public string LogoFileName { get; set; } = string.Empty;
    public TenantStatus TenantStatus { get; set; }
}
