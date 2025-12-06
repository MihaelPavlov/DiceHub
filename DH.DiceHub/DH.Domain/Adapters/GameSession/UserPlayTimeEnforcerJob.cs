using DH.Domain.Queue;

namespace DH.Domain.Adapters.GameSession;

/// <summary>
/// Represents a job specifically for enforcing user playtime requirements for a game.
/// </summary>
/// <param name="UserId">The unique identifier of the user.</param>
/// <param name="GameId">The unique identifier of the game.</param>
/// <param name="AverageTimePlay">The average time that the user is required to play the game.</param>
public record UserPlayTimeEnforcerJob(string UserId, int GameId, DateTime RequiredPlayUntil) 
    : JobInfoBase(GameSessionHelper.BuildJobId(UserId, GameId));
