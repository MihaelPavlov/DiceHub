using DH.Domain.Adapters.PushNotifications.Messages.Common;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class RewardExpirationReminderMessage : MessageRequest
{
    public RewardExpirationReminderMessage(string rewardName, int days)
    {
        Title = $"Your reward is about to expire in {days} days!";
        Body = $"Don't miss out! Your reward '{rewardName}' will expire in {days} days. Visit the club soon to claim it before it's gone!";
    }
}
