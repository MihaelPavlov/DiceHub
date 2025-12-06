using DH.Domain.Enums;

namespace DH.Domain.Adapters.GameSession;

public static class GameSessionHelper
{
    public static string BuildJobId(string userId, int gameId) => $"{userId}-{gameId}";
}
