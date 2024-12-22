namespace DH.Messaging.HttpClient.UserContext;

public class B2bUserContext : IB2bUserContext
{
    public static B2bUserContext Empty { get; } = new B2bUserContext(null, null, null);

    private readonly string? _userId;
    private readonly int? _roleKey;
    private readonly string? _accessToken;

    public B2bUserContext(string? userId, int? roleKey, string? accessToken)
    {
        _userId = userId;
        _roleKey = roleKey;
        _accessToken = accessToken;
    }

    public string UserId
    {
        get
        {
            if (!string.IsNullOrEmpty(_userId))
                return _userId;
            throw new Exception("Can not find current user.");
        }
    }
    public int RoleKey
    {
        get
        {
            if (_roleKey.HasValue)
                return _roleKey.Value;
            throw new Exception("Can not find current user role.");
        }
    }

    public bool IsAuthenticated => !string.IsNullOrEmpty(_userId) && _roleKey.HasValue;

    public string GetAccessToken()
    {
        if (string.IsNullOrEmpty(_accessToken))
            throw new Exception("Can not find accessToken.");
        return _accessToken;
    }
}
