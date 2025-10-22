namespace DH.Domain.Adapters.ChallengeHub;

public interface IChallengeHubClient
{
    Task SendChallengeCompleted(string userId, string challengeGameName, int rewardPoints);
    Task SendChallengeUpdated(string userId, string challengeGameName);
    Task SendUniversalChallengeUpdated(string userId, string challengeName_EN, string challengeName_BG);
    Task SendUniversalChallengeCompleted(string userId, string challengeName_EN, string challengeName_BG, int rewardPoints);
    Task SendUniversalChallengeRestarted(string userId, string challengeName_EN, string challengeName_BG);
    Task SendRewardGranted(string userId, string rewardName_BG, string rewardName_EN);
}
