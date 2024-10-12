using System.Linq.Expressions;

namespace DH.Domain.Repositories;

/// <summary>
/// Defines a repository for performing data operations on entities of the specified type.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public interface IRepository<TEntity>
{
    /// <summary>
    /// Retrieves a list of entities with the specified properties.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="selector">A function to select the properties.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the list of entities with the specified properties.</returns>
    Task<List<TResult>> GetWithPropertiesAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a list of entities with the specified properties.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="wherePredicate">A function to filter the result.</param>
    /// <param name="selector">A function to select the properties.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the list of entities with the specified properties.</returns>
    Task<List<TResult>> GetWithPropertiesAsync<TResult>(
        Expression<Func<TEntity, bool>> wherePredicate,
        Expression<Func<TEntity, TResult>> selector,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a single entity that matches the specified criteria.
    /// </summary>
    /// <param name="selector">A function to define the criteria for selecting the entity.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the entity that matches the specified criteria, or null if no entity matches.</returns>
    Task<TEntity?> GetByAsync(Expression<Func<TEntity, bool>> selector, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a single entity that matches the specified criteria.
    /// </summary>
    /// <param name="selector">A function to define the criteria for selecting the entity.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the entity that matches the specified criteria, or null if no entity matches.</returns>
    Task<TEntity?> GetByAsyncWithTracking(Expression<Func<TEntity, bool>> selector, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously creates a new entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of state entries written to the database.</returns>
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously creates a range of new entities in the repository.
    /// </summary>
    /// <param name="entities">The entities to create.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    Task Update(TEntity entity, CancellationToken cancellationToken);

    /// <summary>
    /// Removes an entity from the repository.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    Task Remove(TEntity entity, CancellationToken cancellationToken);

    /// <summary>
    /// Removes a range of entities from the repository.
    /// </summary>
    /// <param name="entities">The entities to remove.</param>
    Task RemoveRange(IEnumerable<TEntity> entities, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously saves all changes made in the repository.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
