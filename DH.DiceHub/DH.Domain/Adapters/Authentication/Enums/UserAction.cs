namespace DH.Domain.Adapters.Authentication.Enums;

/// <summary>
/// Represents the various actions a user can perform.
/// If you add or modify actions here, 
/// ensure to update the corresponding permissions in the <see cref="MapPermissions"/> class.
/// </summary>
public enum UserAction
{
    GamesRead = 0,
    GamesCUD = 1,
    GameReviewsCRUD = 2,
    GameCategoriesCRUD = 3,
    GameReservedCRUD = 4,
    EventsCUD = 5,
    EventsRead = 6,
    RoomsCRUD = 7,
    ScannerRead = 8,
    SystemRewardsCRUD = 9,
    SystemRewardsRead = 10,
    ChallengesCUD = 11,
    ChallengesRead = 12,
    RewardsRead = 13,
    SpaceManagementCRUD = 14,
    MessagingCRUD = 15,
    SpaceManagementVirtualC = 16,
    NotificationCRUD = 17,
    TenantUserSettingsCRUD = 18,
    TenantSettingsCUD = 19,
    SpaceManagementReservedTablesRU = 20,
    TenantSettingsR = 21,
    EmployeesCRUD = 22,
    EventsAdminRead = 23,
    UsersRead = 24,
}
