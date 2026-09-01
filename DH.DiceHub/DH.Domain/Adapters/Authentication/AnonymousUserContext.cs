namespace DH.Domain.Adapters.Authentication;

public sealed class AnonymousUserContext : IUserContext
{
    public static readonly AnonymousUserContext Instance = new();

    public string? TenantId => null;
    public string? UserId => null;
    public int? RoleKey => null;
    public string? AccessToken => null;
    public string? TimeZone => null;
    public string? Language => null;

    public bool IsSystem => false;
    public bool IsAuthenticated => false;
    private AnonymousUserContext() { }
}
