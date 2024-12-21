namespace DH.Messaging.Publisher.Authentication;

public class RabbitMqUserContextFactory : IRabbitMqUserContextFactory
{
    private IRabbitMqUserContext? _defaultUserContext;

    public IRabbitMqUserContext CreateUserContext()
    {
        return _defaultUserContext ?? throw new InvalidOperationException("User context has not been set.");
    }

    public void SetDefaultUserContext(string userId, bool isAuthenticated, int roleKey, string token)
    {
        _defaultUserContext = new RabbitMqUserContext
        {
            UserId = userId,
            IsAuthenticated = isAuthenticated,
            RoleKey = roleKey,
            Token = token
        };
    }

    public void SetDefaultUserContext(IRabbitMqUserContext defaultUserContext)
    {
        this._defaultUserContext = defaultUserContext;
    }
}

public class RabbitMqUserContext : IRabbitMqUserContext
{
    public string UserId { get; set; } = string.Empty;
    public bool IsAuthenticated { get; set; }
    public int RoleKey { get; set; }
    public string Token { get; set; } = string.Empty;
}
