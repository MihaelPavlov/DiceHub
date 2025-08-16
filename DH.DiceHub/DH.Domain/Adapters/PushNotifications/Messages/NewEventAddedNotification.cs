using DH.Domain.Adapters.Localization;
using DH.Domain.Adapters.PushNotifications.Messages.Common;
using DH.Domain.Adapters.PushNotifications.Messages.Models;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class NewEventAddedNotification : RenderableNotification
{
    public required string EventName { get; set; }
    public required DateTime EventDate { get; set; }

    public override NotificationPayload? Render(
        ILocalizationService localizer,
        string userTimeZone,
        string userLanguage)
    {
        var formattedTime = EventDate.ToUserFormattedString(userTimeZone, userLanguage, localizer);

        return new NotificationPayload
        {
            Title = localizer["NewEventAddedTitle"],  
            Body = string.Format(localizer["NewEventAddedBody"], EventName, formattedTime)
        };
    }
}
