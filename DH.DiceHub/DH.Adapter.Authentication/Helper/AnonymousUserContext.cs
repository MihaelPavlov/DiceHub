using DH.Domain.Adapters.Authentication;

namespace DH.Adapter.Authentication.Helper;

public sealed class AnonymousUserContext : IUserContext
{
    public static readonly AnonymousUserContext Instance = new();

    public string? TenantId => null;
    public string? UserId => null;
    public int? RoleKey => null;
    public string? AccessToken => null;
    public string? TimeZone => null;
    public string? Language => null;

    public bool IsAuthenticated => false;
    private AnonymousUserContext() { }
}
