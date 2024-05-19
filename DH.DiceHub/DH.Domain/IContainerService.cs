namespace DH.Domain;

/// <summary>
/// Provides a method to resolve service instances from a dependency injection container.
/// </summary>
public interface IContainerService
{
    /// <summary>
    /// Resolves an instance of the specified service type.
    /// </summary>
    /// <typeparam name="T">The type of the service to resolve.</typeparam>
    /// <returns>An instance of the service of type <typeparamref name="T"/>.</returns>
    T Resolve<T>();
}