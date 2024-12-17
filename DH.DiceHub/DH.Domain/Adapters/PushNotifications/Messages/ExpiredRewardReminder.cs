using DH.Domain.Adapters.PushNotifications.Messages.Common;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class ExpiredRewardReminder : MessageRequest
{
    public string RewardName { get; set; }
    public DateTime ExpiryDate { get; set; }

    public ExpiredRewardReminder(string rewardName, DateTime expiryDate)
    {
        this.RewardName = rewardName;
        this.ExpiryDate = expiryDate;

        Title = "Reward Expired Soon. Considiring visiting the club?";
        Body = $"Your reward: {this.RewardName} expired on {this.ExpiryDate.ToShortTimeString()}!";
    }
}
