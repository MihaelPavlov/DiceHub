using DH.Domain.Adapters.PushNotifications.Messages.Common;
using DH.Domain.Helpers;
using System.Globalization;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class SpaceTableApprovedMessage : MessageRequest
{
    public SpaceTableApprovedMessage(int numberOfGuests, DateTime reservationDate, bool isUtcFallback)
    {
        var formattedTime = reservationDate.ToString(DateValidator.DATE_TIME_FORMAT, CultureInfo.InvariantCulture);
        if (isUtcFallback)
            formattedTime += " UTC (local time unavailable)";

        Title = "Table Reservation Approved";
        Body = $"Your table reservation for {numberOfGuests} {(numberOfGuests == 1 ? "person" : "people")} at {formattedTime} is COFIRMED!";
    }
}
