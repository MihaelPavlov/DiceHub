using DH.Domain.Adapters.Authentication.Models.Enums;

namespace DH.Domain.Adapters.Authentication.Interfaces;

public interface IMapPermissions
{
    IDictionary<int, List<Role>> GetActionsMapping();
}
