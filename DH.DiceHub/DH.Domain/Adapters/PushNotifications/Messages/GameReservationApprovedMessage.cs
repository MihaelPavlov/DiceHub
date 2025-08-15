using DH.Domain.Adapters.Localization;
using DH.Domain.Adapters.PushNotifications.Messages.Common;
using DH.Domain.Helpers;
using System.Globalization;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class GameReservationApprovedMessage : MessageRequest
{
    public GameReservationApprovedMessage(
        int numberOfGuests, string gameName, DateTime reservationDate,
        bool isUtcFallback, ILocalizationService localizer)
    {
        var formattedTime = reservationDate.ToString(DateValidator.DATE_TIME_FORMAT, CultureInfo.InvariantCulture);
        if (isUtcFallback)
            formattedTime += localizer["UtcFallbackNotice"];

        var peopleWord = numberOfGuests == 1 ? localizer["W_Person"] : localizer["W_People"];

        Title = localizer["GameReservationApprovedTitle"];

        Body = string.Format(
            localizer["GameReservationApprovedBody"],
            gameName,
            numberOfGuests,
            peopleWord,
            formattedTime
        );
    }
}
