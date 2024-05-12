using DH.Domain.Adapters.Authentication;

namespace DH.Adapter.Authentication.Helper;

public class UserContext : IUserContext
{
    readonly string? _userId;

    public UserContext(string? userId)
    {
        _userId = userId;
    }

    public string UserId
    {
        get
        {
            if (_userId != null)
                return _userId;

            throw new Exception("Can not find current user.");
        }
    }

    public bool IsAuthenticated => _userId != null;
}
