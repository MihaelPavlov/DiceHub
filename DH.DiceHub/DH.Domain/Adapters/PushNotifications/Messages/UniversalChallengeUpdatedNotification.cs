using DH.Domain.Adapters.Localization;
using DH.Domain.Adapters.PushNotifications.Messages.Common;
using DH.Domain.Adapters.PushNotifications.Messages.Models;
using DH.Domain.Enums;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class UniversalChallengeUpdatedNotification : RenderableNotification
{
    public required string ChallengeName_EN { get; set; }
    public required string ChallengeName_BG { get; set; }

    public override NotificationPayload? Render(ILocalizationService localizer, string userTimeZone, string userLanguage)
    {
        string challengeName = userLanguage == SupportLanguages.EN.ToString() ? ChallengeName_EN : ChallengeName_BG;

        return new NotificationPayload
        {
            Title = localizer["ChallengeUpdatedTitle"],
            Body = string.Format(localizer["UniversalChallengeUpdatedBody"], challengeName)
        };
    }
}
