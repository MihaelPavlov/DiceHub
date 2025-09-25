using DH.Domain.Adapters.Localization;
using DH.Domain.Adapters.PushNotifications.Messages.Common;
using DH.Domain.Adapters.PushNotifications.Messages.Models;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class ChallengeUpdatedNotification : RenderableNotification
{
    public required string ChallengeName { get; set; }

    public override NotificationPayload? Render(ILocalizationService localizer, string userTimeZone, string userLanguage)
    {
        return new NotificationPayload
        {
            Title = localizer["ChallengeUpdatedTitle"],
            Body = string.Format(localizer["ChallengeUpdatedBody"], ChallengeName)
        };
    }
}