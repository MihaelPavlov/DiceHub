namespace DH.Domain.Adapters.Data;

public interface ITenantDbContext
{
    T AcquireRepository<T>();
}
