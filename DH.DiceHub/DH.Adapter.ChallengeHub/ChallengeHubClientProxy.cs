using DH.Domain.Adapters.ChallengeHub;
using DH.Domain.Adapters.PushNotifications;
using Microsoft.AspNetCore.SignalR;

namespace DH.Adapter.ChallengeHub;

public class ChallengeHubClientProxy : IChallengeHubClient
{
    readonly IHubContext<ChallengeHubClient> hub;
    readonly IPushNotificationsService pushNotifications;

    public ChallengeHubClientProxy(IHubContext<ChallengeHubClient> hub, IPushNotificationsService pushNotifications)
    {
        this.hub = hub;
        this.pushNotifications = pushNotifications;
    }

    public Task SendChallengeCompleted(string userId, string challengeGameName, int rewardPoints)
    {
        return new ChallengeHubClient(hub, pushNotifications)
            .SendChallengeCompleted(userId, challengeGameName, rewardPoints);
    }

    public Task SendChallengeUpdated(string userId, string challengeGameName)
    {
        return new ChallengeHubClient(hub, pushNotifications)
            .SendChallengeUpdated(userId, challengeGameName);
    }

    public Task SendRewardGranted(string userId, string rewardName_BG, string rewardName_EN)
    {
        return new ChallengeHubClient(hub, pushNotifications)
            .SendRewardGranted(userId, rewardName_BG, rewardName_EN);
    }

    public Task SendUniversalChallengeCompleted(string userId, string challengeName_EN, string challengeName_BG, int rewardPoints)
    {
        return new ChallengeHubClient(hub, pushNotifications)
         .SendUniversalChallengeCompleted(userId, challengeName_EN, challengeName_BG, rewardPoints);
    }

    public Task SendUniversalChallengeRestarted(string userId, string challengeName_EN, string challengeName_BG)
    {
        return new ChallengeHubClient(hub, pushNotifications)
         .SendUniversalChallengeRestarted(userId, challengeName_EN, challengeName_BG);
    }

    public Task SendUniversalChallengeUpdated(string userId, string challengeName_EN, string challengeName_BG)
    {
        return new ChallengeHubClient(hub, pushNotifications)
         .SendUniversalChallengeUpdated(userId, challengeName_EN, challengeName_BG);
    }
}
