using DH.Domain.Adapters.Authentication;

namespace DH.Adapter.Authentication.Helper;

public class VirtualUserContextFactory : IUserContextFactory
{
    IUserContext _defaultUserContext = new UserContext("1", 0);

    public IUserContext CreateUserContext()
    {
        return _defaultUserContext;
    }

    public void SetDefaultUserContext(IUserContext defaultUserContext)
    {
        _defaultUserContext = defaultUserContext;
    }
}
