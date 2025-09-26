using DH.Domain.Adapters.Localization;
using DH.Domain.Adapters.PushNotifications.Messages.Common;
using DH.Domain.Adapters.PushNotifications.Messages.Models;
using DH.Domain.Enums;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class RewardGrantedNotification : RenderableNotification
{
    public required string RewardName_EN { get; set; }
    public required string RewardName_BG { get; set; }

    public override NotificationPayload? Render(ILocalizationService localizer, string userTimeZone, string userLanguage)
    {
        string rewardName = userLanguage == SupportLanguages.EN.ToString() ? RewardName_EN : RewardName_BG;

        return new NotificationPayload
        {
            Title = localizer["RewardGrantedTitle"],
            Body = string.Format(localizer["RewardGrantedBody"], rewardName)
        };
    }
}
