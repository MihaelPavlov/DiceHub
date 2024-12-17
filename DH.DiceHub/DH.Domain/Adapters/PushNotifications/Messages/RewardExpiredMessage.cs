using DH.Domain.Adapters.PushNotifications.Messages.Common;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class RewardExpiredMessage : MessageRequest
{
    public RewardExpiredMessage(string rewardName)
    {
        Title = "Reward Expired. Don't worry visit the club for more awesome rewards?";
        Body = $"Unfortunately your reward has expired: {rewardName} !";
    }
}
