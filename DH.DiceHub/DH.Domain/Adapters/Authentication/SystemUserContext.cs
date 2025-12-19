using DH.Domain.Adapters.Authentication;

namespace DH.Adapter.Authentication.Helper;

public sealed class SystemUserContext : IUserContext
{
    public SystemUserContext(
        string tenantId,
        string userId,
        int? roleKey = null,
        string? timeZone = "UTC",
        string? language = "en"
    )
    {
        TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        RoleKey = roleKey;
        TimeZone = timeZone;
        Language = language;
        IsAuthenticated = false;
    }
    public string? TenantId { get; init; }
    public string? UserId { get; init; }
    public int? RoleKey { get; init; }
    public string? TimeZone { get; init; }
    public string? Language { get; init; }
    public bool IsAuthenticated { get; init; }
    public bool IsSystem => true;
}
