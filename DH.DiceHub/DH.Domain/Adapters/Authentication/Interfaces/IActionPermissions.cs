
namespace DH.Domain.Adapters.Authentication.Interfaces;

public interface IActionPermissions<T> where T : Enum
{
    bool Has(T action, IUserContext userContext);
}

