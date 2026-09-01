using DH.Domain.Adapters.Authentication;

/// <summary>
/// Immutable implementation of <see cref="IUserContext"/>.
/// Safe for anonymous, system, and authenticated users.
/// </summary>
public sealed class UserContext : IUserContext
{
    public string? TenantId { get; }
    public string? UserId { get; }
    public int? RoleKey { get; }
    public string? TimeZone { get; }
    public string? Language { get; }
    public bool IsSystem { get; }

    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(UserId) && !string.IsNullOrWhiteSpace(TenantId);

    public UserContext(
        string? tenantId,
        string? userId,
        int? roleKey,
        string? timeZone,
        string? language)
    {
        TenantId = tenantId;
        UserId = userId;
        RoleKey = roleKey;
        TimeZone = timeZone;
        Language = language;
        IsSystem = false;
    }
}
