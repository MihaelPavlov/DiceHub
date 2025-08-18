using DH.Domain.Adapters.Localization;
using DH.Domain.Adapters.PushNotifications.Messages.Common;
using DH.Domain.Adapters.PushNotifications.Messages.Models;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class RoomParticipantJoinedNotification : RenderableNotification
{
    public required string RoomName { get; set; }
    public required string ParticipantName { get; set; }

    public override NotificationPayload? Render(ILocalizationService localizer, string userTimeZone, string userLanguage)
    {
        return new NotificationPayload
        {
            Title = string.Format( localizer["RoomParticipantJoinedTitle"], RoomName),
            Body = string.Format(localizer["RoomParticipantJoinedBody"], ParticipantName, RoomName)
        };
    }
}