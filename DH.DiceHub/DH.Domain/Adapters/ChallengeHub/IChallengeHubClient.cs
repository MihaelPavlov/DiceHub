namespace DH.Domain.Adapters.ChallengeHub;

public interface IChallengeHubClient
{
    Task SendChallengeCompleted(string userId, string challengeGameName, int rewardPoints);
    Task SendChallengeUpdated(string userId, string challengeGameName);
    Task SendRewardGranted(string userId, string rewardName_BG, string rewardName_EN);
}
