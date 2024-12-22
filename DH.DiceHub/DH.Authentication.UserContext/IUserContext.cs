namespace DH.Messaging.HttpClient.UserContext;

public interface IUserContext
{
    string UserId { get; }
    int RoleKey { get; }
    string GetAccessToken();
    bool IsAuthenticated { get; }
}
