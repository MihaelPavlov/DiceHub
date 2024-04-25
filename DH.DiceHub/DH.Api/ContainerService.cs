using DH.Domain;

namespace DH.Api;

public class ContainerService : IContainerService
{
    private readonly IServiceProvider _serviceProvider;

    public ContainerService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public T Resolve<T>()
    {
        return _serviceProvider.GetRequiredService<T>();
    }
}
