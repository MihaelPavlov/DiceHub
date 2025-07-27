using DH.Domain.Adapters.PushNotifications.Messages.Common;
using DH.Domain.Helpers;
using System.Globalization;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class GameReservationDeclinedMessage : MessageRequest
{
    public GameReservationDeclinedMessage(int numberOfGuests, string gameName, DateTime reservationDate, bool isUtcFallback)
    {
        var formattedTime = reservationDate.ToString(DateValidator.DATE_TIME_FORMAT, CultureInfo.InvariantCulture);
        if (isUtcFallback)
            formattedTime += " UTC (local time unavailable)";

        Title = "Game Reservation DECLINED";
        Body = $"Your reservation for game {gameName} and {numberOfGuests} {(numberOfGuests == 1 ? "person" : "people")} at {formattedTime} is DECLINED!";
    }
}
