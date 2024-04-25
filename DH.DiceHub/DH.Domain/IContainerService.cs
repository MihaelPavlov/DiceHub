namespace DH.Domain;

public interface IContainerService
{
    T Resolve<T>();
}
