using DH.Domain.Adapters.Authentication;

namespace DH.Adapter.Authentication.Helper;

/// <summary>
/// A factory class for creating instances of <see cref="IUserContext"/> that simulates a default user context.
/// This is useful for testing or migration utilities where a real user context is not required.
/// </summary>
public class VirtualUserContextFactory : IUserContextFactory
{
    IUserContext _defaultUserContext = new UserContext("0", 0);

    public IUserContext CreateUserContext()
    {
        return _defaultUserContext;
    }

    public void SetDefaultUserContext(IUserContext defaultUserContext)
    {
        _defaultUserContext = defaultUserContext;
    }
}
