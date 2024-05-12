namespace DH.Domain.Adapters.Authentication;

public interface IUserContext
{
    string UserId { get; }
    bool IsAuthenticated { get; }
}
