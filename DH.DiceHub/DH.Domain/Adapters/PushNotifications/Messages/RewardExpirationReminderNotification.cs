using DH.Domain.Adapters.Localization;
using DH.Domain.Adapters.PushNotifications.Messages.Common;
using DH.Domain.Adapters.PushNotifications.Messages.Models;
using DH.Domain.Enums;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class RewardExpirationReminderNotification : RenderableNotification
{
    public required string RewardName_EN { get; set; }
    public required string RewardName_BG { get; set; }
    public required int Days { get; set; }

    public override NotificationPayload? Render(
        ILocalizationService localizer,
        string userTimeZone,
        string userLanguage)
    {
        string dayWord = Days == 1 ? localizer["Day_Singular"] : localizer["Day_Plural"];
        string rewardName = userLanguage == SupportLanguages.EN.ToString() ? RewardName_EN : RewardName_BG;

        return new NotificationPayload
        {
            Title = string.Format(localizer["RewardExpirationReminderTitle"], Days, dayWord),
            Body = string.Format(localizer["RewardExpirationReminderBody"], rewardName, Days, dayWord)
        };
    }
}
