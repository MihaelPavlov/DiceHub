namespace DH.Domain.Adapters.Authentication;

/// <summary>
/// Provides a method to acquire a repository instance.
/// </summary>
public interface IIdentityDbContext
{
    /// <summary>
    /// Retrieves an instance of a repository of the specified type.
    /// This interface is designed to abstract the context for identity-related data operations, allowing the retrieval of repository instances for various entity types.
    /// </summary>
    /// <typeparam name="T">The type of the repository to acquire.</typeparam>
    /// <returns>An instance of the repository of type <typeparamref name="T"/>.</returns>
    T AcquireRepository<T>();
}
