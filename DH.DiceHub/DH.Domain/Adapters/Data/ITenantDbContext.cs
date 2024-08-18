using DH.Domain.Repositories;

namespace DH.Domain.Adapters.Data;

/// <summary>
/// Provides a method to acquire a repository instance specific to tenant-related data operations.
/// </summary>
public interface ITenantDbContext : IDisposable
{
    /// <summary>
    /// Retrieves an instance of a repository of the specified type.
    /// This interface is designed to abstract the context for tenant-related data operations, allowing the retrieval of repository instances for various entity types.
    /// </summary>
    /// <typeparam name="T">The type of the repository to acquire.</typeparam>
    /// <returns>An instance of the repository of type <typeparamref name="T"/>.</returns>
    T AcquireRepository<T>();

    /// <summary>
    /// Asynchronously saves all changes made in this context to the underlying database.
    /// </summary>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
