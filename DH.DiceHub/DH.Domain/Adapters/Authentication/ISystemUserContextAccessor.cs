namespace DH.Domain.Adapters.Authentication;

public interface ISystemUserContextAccessor
{
    IUserContext Current { get; }
    void Set(IUserContext context);
}
