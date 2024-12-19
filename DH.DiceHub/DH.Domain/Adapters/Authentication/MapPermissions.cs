using DH.Domain.Adapters.Authentication.Enums;
using DH.Domain.Adapters.Authentication.Interfaces;
using DH.Domain.Adapters.Authentication.Models.Enums;

namespace DH.Domain.Adapters.Authentication;

/// <summary>
/// Maps user actions to their allowed roles
/// </summary>
public class MapPermissions : IMapPermissions, IActionPermissions<UserAction>
{
    /// <summary>
    /// Dictionary that maps each <see cref="UserAction"/> to a list of roles permitted to perform the action.
    /// </summary>
    readonly static Dictionary<UserAction, List<Role>> _map = new()
    {
        { UserAction.GamesRead, new List<Role> { Role.SuperAdmin, Role.Owner, Role.Staff, Role.User} },
        { UserAction.GamesCUD, new List<Role> { Role.SuperAdmin, Role.Owner, Role.Staff} },
        { UserAction.GameReviewsCRUD, new List<Role> { Role.SuperAdmin, Role.Owner, Role.Staff, Role.User} },
        { UserAction.GameCategoriesCRUD, new List<Role> { Role.SuperAdmin, Role.Owner, Role.Staff, Role.User} },
        { UserAction.GameReservedCRUD, new List<Role> { Role.SuperAdmin, Role.Owner, Role.Staff} },
        { UserAction.EventsCUD, new List<Role> { Role.SuperAdmin, Role.Owner, Role.Staff} },
        { UserAction.EventsRead, new List<Role> { Role.SuperAdmin, Role.Owner, Role.Staff, Role.User} },
        { UserAction.RoomsCRUD, new List<Role> { Role.SuperAdmin, Role.Owner, Role.Staff, Role.User} },
        { UserAction.ScannerRead, new List<Role> { Role.SuperAdmin, Role.Owner, Role.Staff, Role.User} },
        { UserAction.SystemRewardsCRUD, new List<Role> { Role.SuperAdmin, Role.Owner, Role.Staff} },
        { UserAction.SystemRewardsRead, new List<Role> { Role.SuperAdmin, Role.Owner, Role.Staff, Role.User} },
        { UserAction.ChallengesCUD, new List<Role> { Role.SuperAdmin, Role.Owner, Role.Staff} },
        { UserAction.ChallengesRead, new List<Role> { Role.SuperAdmin, Role.Owner, Role.Staff, Role.User} },
        { UserAction.RewardsRead, new List<Role> { Role.SuperAdmin, Role.Owner, Role.Staff, Role.User} },
        { UserAction.SpaceManagementCRUD, new List<Role> { Role.SuperAdmin, Role.Owner, Role.Staff, Role.User} },
        { UserAction.SpaceManagementReservedTablesRU, new List<Role> { Role.SuperAdmin, Role.Owner, Role.Staff} },
        { UserAction.MessagingCRUD, new List<Role> { Role.SuperAdmin, Role.Owner, Role.Staff, Role.User} },
        { UserAction.SpaceManagementVirtualC, new List<Role> { Role.SuperAdmin, Role.Owner, Role.Staff} },
        { UserAction.NotificationCRUD, new List<Role> { Role.SuperAdmin, Role.Owner, Role.Staff, Role.User} },
        { UserAction.TenantUserSettingsCRUD, new List<Role> { Role.SuperAdmin, Role.Owner, Role.Staff, Role.User} },
        { UserAction.TenantSettingsCUD, new List<Role> { Role.SuperAdmin, Role.Owner} },
        { UserAction.TenantSettingsR, new List<Role> { Role.SuperAdmin, Role.Owner, Role.Staff, Role.User} },
        { UserAction.EmployeesCRUD, new List<Role> { Role.SuperAdmin, Role.Owner} },
    };

    public IDictionary<int, List<Role>> GetActionsMapping()
    {
        return _map.ToDictionary(x => (int)x.Key, x => x.Value);
    }

    public bool Has(UserAction action, IUserContext userContext)
    {
        if (!_map.TryGetValue(action, out var roles))
            return false;

        return roles.Cast<int>().Contains(userContext.RoleKey);
    }
}