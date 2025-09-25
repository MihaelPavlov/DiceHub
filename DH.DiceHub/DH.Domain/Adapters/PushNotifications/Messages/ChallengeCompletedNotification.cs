using DH.Domain.Adapters.Localization;
using DH.Domain.Adapters.PushNotifications.Messages.Common;
using DH.Domain.Adapters.PushNotifications.Messages.Models;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class ChallengeCompletedNotification : RenderableNotification
{
    public required string ChallengeName { get; set; }
    public required int RewardPoints { get; set; }

    public override NotificationPayload? Render(ILocalizationService localizer, string userTimeZone, string userLanguage)
    {
        return new NotificationPayload
        {
            Title = localizer["ChallengeCompletedTitle"],
            Body = string.Format(localizer["ChallengeCompletedBody"], ChallengeName, RewardPoints)
        };
    }
}
