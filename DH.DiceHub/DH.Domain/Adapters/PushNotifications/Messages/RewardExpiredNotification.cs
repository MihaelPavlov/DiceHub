using DH.Domain.Adapters.Localization;
using DH.Domain.Adapters.PushNotifications.Messages.Common;
using DH.Domain.Adapters.PushNotifications.Messages.Models;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class RewardExpiredNotification : RenderableNotification
{
    public required string RewardName { get; set; }

    public override NotificationPayload? Render(ILocalizationService localizer, string userTimeZone, string userLanguage)
    {
        return new NotificationPayload
        {
            Title = localizer["RewardExpiredTitle"],
            Body = string.Format(localizer["RewardExpiredBody"], RewardName)
        };
    }
}
