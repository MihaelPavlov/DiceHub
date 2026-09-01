namespace DH.Domain.Adapters.Authentication;

public interface ISystemUserContextAccessor
{
    IUserContext Current { get; }
    IUserContext Peek { get; }
    void Set(IUserContext context);
}
