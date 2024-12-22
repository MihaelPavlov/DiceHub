namespace DH.Messaging.HttpClient.UserContext;

public interface IB2bUserContext
{
    string UserId { get; }
    int RoleKey { get; }
    string GetAccessToken();
    bool IsAuthenticated { get; }
}
