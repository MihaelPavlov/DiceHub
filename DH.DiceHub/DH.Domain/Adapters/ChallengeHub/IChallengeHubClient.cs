namespace DH.Domain.Adapters.ChallengeHub;

public interface IChallengeHubClient
{
    Task SendChallengeUpdated(string userId, int challengeId);
    Task SendChallengeCompleted(string userId, int challengeId);
}
