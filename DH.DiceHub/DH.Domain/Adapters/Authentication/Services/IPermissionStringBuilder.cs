namespace DH.Domain.Adapters.Authentication.Services;

public interface IPermissionStringBuilder
{
    string BuildPermissionsString(int roleKey);
    string GetFromCacheOrBuildPermissionsString(int roleKey);
}
