using DH.Domain.Adapters.Localization;
using DH.Domain.Adapters.PushNotifications.Messages.Common;
using DH.Domain.Adapters.PushNotifications.Messages.Models;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class RoomGameChangedNotification : RenderableNotification
{
    public required string RoomName { get; set; }
    public required string OldGameName { get; set; }
    public required string NewGameName { get; set; }

    public override NotificationPayload? Render(ILocalizationService localizer, string userTimeZone, string userLanguage)
    {
        return new NotificationPayload
        {
            Title = string.Format(localizer["RoomGameChangedTitle"], RoomName),
            Body = string.Format(
                localizer["RoomGameChangedBody"],
                RoomName,
                OldGameName,
                NewGameName
            )
        };
    }
}
