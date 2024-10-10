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
    SystemRewardCRUD = 9,
    SystemRewardRead = 10,
    ChallengesCUD = 11,
    ChallengesRead = 12,
    RewardRead = 13,
}
