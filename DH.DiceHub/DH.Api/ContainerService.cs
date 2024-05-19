using DH.Domain;

namespace DH.Api;

/// <summary>
/// Implements the <see cref="IContainerService"/> to resolve service instances from a dependency injection container.
/// </summary>
public class ContainerService : IContainerService
{
    readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContainerService"/> class with the specified service provider.
    /// </summary>
    /// <param name="serviceProvider">The service provider to use for resolving services.</param>
    public ContainerService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    public T Resolve<T>()
    {
        return _serviceProvider.GetRequiredService<T>();
    }
}
