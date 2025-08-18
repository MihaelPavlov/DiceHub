using DH.Domain.Adapters.Localization;
using DH.Domain.Adapters.PushNotifications.Messages.Common;
using DH.Domain.Adapters.PushNotifications.Messages.Models;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class RoomStartDateChangedNotification : RenderableNotification
{
    public required string RoomName { get; set; }
    public required DateTime OldDate { get; set; }
    public required DateTime NewDate { get; set; }

    public override NotificationPayload? Render(ILocalizationService localizer, string userTimeZone, string userLanguage)
    {
        var oldDateFormatted = OldDate.ToUserFormattedString(userTimeZone, userLanguage, localizer);
        var newDateFormatted = NewDate.ToUserFormattedString(userTimeZone, userLanguage, localizer);

        return new NotificationPayload
        {
            Title = string.Format(localizer["RoomStartDateChangedTitle"], RoomName),
            Body = string.Format(
                localizer["RoomStartDateChangedBody"],
                RoomName,
                oldDateFormatted,
                newDateFormatted
            )
        };
    }
}