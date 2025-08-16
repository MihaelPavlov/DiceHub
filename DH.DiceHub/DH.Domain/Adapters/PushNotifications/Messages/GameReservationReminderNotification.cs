using DH.Domain.Adapters.Localization;
using DH.Domain.Adapters.PushNotifications.Messages.Common;
using DH.Domain.Adapters.PushNotifications.Messages.Models;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class GameReservationReminderNotification : RenderableNotification
{
    public required string GameName { get; set; }
    public required string Email { get; set; }
    public required int CountPeople { get; set; }
    public required DateTime ReservationTime { get; set; }

    public override NotificationPayload Render(ILocalizationService localizer, string userTimeZone, string userLanguage)
    {
        var peopleWord = localizer.PersonWord(CountPeople);
        var formattedTime = ReservationTime.ToUserFormattedString(userTimeZone, userLanguage, localizer);

        return new NotificationPayload
        {
            Title = localizer["NewGameReservationReminderTitle"],
            Body = string.Format(localizer["NewGameReservationReminderBody"],
                Email, CountPeople, peopleWord, GameName, formattedTime)
        };
    }
}