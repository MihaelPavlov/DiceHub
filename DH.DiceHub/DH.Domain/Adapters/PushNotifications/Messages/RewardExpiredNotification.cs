using DH.Domain.Adapters.Localization;
using DH.Domain.Adapters.PushNotifications.Messages.Common;
using DH.Domain.Adapters.PushNotifications.Messages.Models;
using DH.Domain.Enums;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class RewardExpiredNotification : RenderableNotification
{
    public required string RewardName_EN { get; set; }
    public required string RewardName_BG { get; set; }

    public override NotificationPayload? Render(ILocalizationService localizer, string userTimeZone, string userLanguage)
    {
        var rewardName = userLanguage == SupportLanguages.EN.ToString() ? RewardName_EN : RewardName_BG;

        return new NotificationPayload
        {
            Title = localizer["RewardExpiredTitle"],
            Body = string.Format(localizer["RewardExpiredBody"], rewardName)
        };
    }
}
