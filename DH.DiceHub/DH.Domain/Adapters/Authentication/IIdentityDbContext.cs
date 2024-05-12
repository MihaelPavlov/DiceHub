namespace DH.Domain.Adapters.Authentication;

public interface IIdentityDbContext
{
    T AcquireRepository<T>();
}
