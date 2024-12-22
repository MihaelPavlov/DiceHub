namespace DH.Messaging.HttpClient.UserContext;

public interface IB2bUserContextFactory
{

    public IB2bUserContext CreateUserContext();
    void SetDefaultUserContext(IB2bUserContext defaultUserContext);
}
