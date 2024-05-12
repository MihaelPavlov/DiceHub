namespace DH.Domain.Adapters.Authentication;

public interface IUserContextFactory
{
    public IUserContext CreateUserContext();
}
