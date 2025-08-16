using DH.Domain.Adapters.Localization;
using DH.Domain.Adapters.PushNotifications.Messages.Common;
using DH.Domain.Adapters.PushNotifications.Messages.Models;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class SpaceTableApprovedNotification : RenderableNotification
{
    public required int NumberOfGuests { get; set; }
    public required DateTime ReservationDate { get; set; }

    public override NotificationPayload? Render(ILocalizationService localizer, string userTimeZone, string userLanguage)
    {
        var peopleWord = localizer.PersonWord(NumberOfGuests);
        var formattedTime = ReservationDate.ToUserFormattedString(userTimeZone, userLanguage, localizer);

        return new NotificationPayload
        {
            Title = localizer["SpaceTableApprovedTitle"],
            Body = string.Format(localizer["SpaceTableApprovedBody"], NumberOfGuests, peopleWord, formattedTime)
        };
    }
}