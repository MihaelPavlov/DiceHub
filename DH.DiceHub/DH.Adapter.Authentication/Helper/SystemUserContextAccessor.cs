using DH.Domain.Adapters.Authentication;

namespace DH.Adapter.Authentication.Helper;

public class SystemUserContextAccessor : ISystemUserContextAccessor
{
    private IUserContext _current = AnonymousUserContext.Instance;

    public IUserContext Current
    {
        get
        {
            var context = _current;
            _current = AnonymousUserContext.Instance;
            return context;
        }
    }

    public void Set(IUserContext context)
    {
        _current = context;
    }
}
