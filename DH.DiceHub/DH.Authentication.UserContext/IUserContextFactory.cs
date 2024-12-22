namespace DH.Messaging.HttpClient.UserContext;

public interface IUserContextFactory
{
    public IUserContext CreateUserContext();
    void SetDefaultUserContext(IUserContext defaultUserContext);
}
