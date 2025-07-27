using DH.Domain.Adapters.PushNotifications.Messages.Common;
using DH.Domain.Helpers;
using System.Globalization;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class SpaceTablePublicNoteUpdatedMessage : MessageRequest
{
    public SpaceTablePublicNoteUpdatedMessage(int numberOfGuests, DateTime reservationDate, bool isUtcFallback)
    {
        var formattedTime = reservationDate.ToString(DateValidator.DATE_TIME_FORMAT, CultureInfo.InvariantCulture);
        if (isUtcFallback)
            formattedTime += " UTC (local time unavailable)";

        Title = "Table Reservation Note Changed!";
        Body = $"Your table reservation for {numberOfGuests} {(numberOfGuests == 1 ? "person" : "people")} at {formattedTime} have a new note, please review it!";
    }
}
