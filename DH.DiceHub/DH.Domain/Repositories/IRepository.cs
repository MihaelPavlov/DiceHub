namespace DH.Domain.Repositories;

public interface IRepository<TEntity>
{
    Task<int> CreateAsync(TEntity entity, CancellationToken cancellationToken);
}
