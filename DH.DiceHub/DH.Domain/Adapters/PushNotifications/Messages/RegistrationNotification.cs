using DH.Domain.Adapters.Localization;
using DH.Domain.Adapters.PushNotifications.Messages.Common;
using DH.Domain.Adapters.PushNotifications.Messages.Models;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class RegistrationNotification : RenderableNotification
{
    public required string Username { get; set; }
    public required string ClubName { get; set; }

    public override NotificationPayload? Render(
        ILocalizationService localizer,
        string userTimeZone,
        string userLanguage)
    {
        return new NotificationPayload
        {
            Title = string.Format(localizer["RegistrationMessageTitle"], ClubName),
            Body = string.Format(localizer["RegistrationMessageBody"], Username)
        };
    }
}
