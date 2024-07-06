using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Authentication.Services;

namespace DH.Adapter.Authentication.Services;

public class UserActionService : IUserActionService
{
    readonly IUserContext _user;
    readonly IPermissionStringBuilder _permissionStringBuilder;

    public UserActionService(IUserContext user, IPermissionStringBuilder permissionStringBuilder)
    {
        _user = user;
        _permissionStringBuilder = permissionStringBuilder;
    }

    public bool IsActionAvailable(int actionKey)
    {
        var permissionString = _permissionStringBuilder.GetFromCacheOrBuildPermissionsString(_user.RoleKey);
        return permissionString[actionKey] == '1';
    }
}
