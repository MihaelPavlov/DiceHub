namespace DH.Domain.Adapters.Authentication.Models.Enums;

public enum Role
{
    SuperAdmin = 0,
    Owner = 1,
    Staff = 2,
    User = 3
}

public static class RoleHelper
{
    public static int GetRoleKeyByName(string roleName)
    {
        if (Enum.TryParse(typeof(Role), roleName, true, out var role))
        {
            return (int)role;
        }
        throw new ArgumentException("Invalid role name", nameof(roleName));
    }
}