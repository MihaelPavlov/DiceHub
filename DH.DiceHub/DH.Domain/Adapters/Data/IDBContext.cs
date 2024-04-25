namespace DH.Domain.Adapters.Data;

public interface IDBContext
{
    T AcquireRepository<T>();
}
