using DH.Domain.Adapters.Authentication;

namespace DH.Adapter.Authentication.Helper;

/// <summary>
/// A factory class for creating instances of <see cref="IUserContext"/> that simulates a default user context.
/// This is useful for testing or migration utilities where a real user context is not required.
/// </summary>
public class VirtualUserContextFactory : IUserContextFactory
{
    IUserContext _defaultUserContext = new UserContext("0", 0, string.Empty);

    public IUserContext CreateUserContext()
    {
        return _defaultUserContext;
    }

    public IUserContext GetUserContextForB2b()
    {
        throw new NotImplementedException("No B2B Comunication inside the virtual user context");
    }

    public void SetDefaultUserContext(IUserContext defaultUserContext)
    {
        _defaultUserContext = defaultUserContext;
    }
}
