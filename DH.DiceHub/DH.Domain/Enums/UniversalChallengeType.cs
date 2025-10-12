using System.Runtime.Serialization;

namespace DH.Domain.Enums;

public enum UniversalChallengeType
{
    [EnumMember(Value = "Play Games")]
    PlayGames,

    [EnumMember(Value = "Join Meeple Rooms")]
    JoinMeepleRooms,

    [EnumMember(Value = "Join Events")]
    JoinEvents,

    [EnumMember(Value = "Use Rewards")]
    UseRewards,

    [EnumMember(Value = "Rewards Granted")]
    RewardsGranted,

    [EnumMember(Value = "Buy Items")]
    BuyItems,

    [EnumMember(Value = "Play Favorite Game")]
    PlayFavoriteGame,

    [EnumMember(Value = "Top 3 Challenge Leaderboard")]
    Top3ChallengeLeaderboard,

    [EnumMember(Value = "Top 3 Streak Leaderboard")]
    Top3StreakLeaderboard
}
