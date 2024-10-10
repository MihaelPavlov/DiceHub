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
        { UserAction.SystemRewardCRUD, new List<Role> { Role.SuperAdmin, Role.Owner, Role.Staff} },
        { UserAction.SystemRewardRead, new List<Role> { Role.SuperAdmin, Role.Owner, Role.Staff, Role.User} },
        { UserAction.ChallengesCUD, new List<Role> { Role.SuperAdmin, Role.Owner, Role.Staff} },
        { UserAction.ChallengesRead, new List<Role> { Role.SuperAdmin, Role.Owner, Role.Staff, Role.User} },
        { UserAction.RewardRead, new List<Role> { Role.SuperAdmin, Role.Owner, Role.Staff, Role.User} },
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