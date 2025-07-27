using DH.Domain.Adapters.PushNotifications.Messages.Common;
using DH.Domain.Helpers;
using System.Globalization;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class SpaceTableReservationManagementReminder : MessageRequest
{
    public SpaceTableReservationManagementReminder(int countPeople, DateTime reservationDate, bool isUtcFallback)
    {
        var formattedTime = reservationDate.ToString(DateValidator.DATE_TIME_FORMAT, CultureInfo.InvariantCulture);
        if (isUtcFallback)
            formattedTime += " UTC (local time unavailable)";

        Title = "New Table Reservation Reminder";
        Body = $"You are having a new table reservation. Reserve a table for {countPeople} {(countPeople == 1 ? "person" : "people")} at {formattedTime}!";
    }
}
