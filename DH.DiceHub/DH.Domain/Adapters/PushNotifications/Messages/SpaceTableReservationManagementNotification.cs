using DH.Domain.Adapters.Localization;
using DH.Domain.Adapters.PushNotifications.Messages.Common;
using DH.Domain.Adapters.PushNotifications.Messages.Models;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class SpaceTableReservationManagementNotification : RenderableNotification
{
    public required int CountPeople { get; set; }
    public required DateTime ReservationDate { get; set; }

    public override NotificationPayload? Render(ILocalizationService localizer, string userTimeZone, string userLanguage)
    {
        var peopleWord = localizer.PersonWord(CountPeople);
        var formattedTime = ReservationDate.ToUserFormattedString(userTimeZone, userLanguage, localizer);

        return new NotificationPayload
        {
            Title = localizer["SpaceTableReservationReminderTitle"],
            Body = string.Format(localizer["SpaceTableReservationReminderBody"], CountPeople, peopleWord, formattedTime)
        };
    }
}
