using DH.Domain.Adapters.PushNotifications.Messages.Common;
using DH.Domain.Helpers;
using System.Globalization;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class EventReminder : MessageRequest
{
    public EventReminder(string eventName, DateTime eventDate, bool isUtcFallback)
    {
        var formattedTime = eventDate.ToString(DateValidator.DATE_TIME_FORMAT, CultureInfo.InvariantCulture);
        if (isUtcFallback)
            formattedTime += " UTC (local time unavailable)";

        Title = "Reminder: Upcoming Event";
        Body = $"Don't forget! You're registered for the event **{eventName}** happening on {formattedTime}. We look forward to seeing you there!";
    }
}
