using DH.Adapter.Authentication.Entities;

namespace DH.Adapter.Authentication.Helper;

internal static class UserExtensions
{
    internal static bool IsInvalid(this ApplicationUser? user)
    => user == null || user.IsDeleted;
}
