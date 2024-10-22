using DH.Domain.Adapters.PushNotifications.Messages.Common;

namespace DH.Domain.Adapters.PushNotifications.Messages;

internal class ChallengeCompletionReminder : MessageRequest
{
    public string ChallengeName { get; set; }

    public ChallengeCompletionReminder(string challengeName)
    {
        this.ChallengeName = challengeName;

        Title = "Completed Challenge";
        Body = $"Challenge: {this.ChallengeName} is completed. Visit your challenge page!";
    }
}
