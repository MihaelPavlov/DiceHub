using DH.Domain.Adapters.PushNotifications.Messages.Common;
using DH.Domain.Helpers;
using System.Globalization;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class EventStartDateUpdateMessage : MessageRequest
{
    public EventStartDateUpdateMessage(string eventName, DateTime eventDate, bool isUtcFallback)
    {
        var formattedTime = eventDate.ToString(DateValidator.DATE_TIME_FORMAT, CultureInfo.InvariantCulture);
        if (isUtcFallback)
            formattedTime += " UTC (local time unavailable)";

        Title = "Event Start Date Updated";
        Body = $"The start date for **{eventName}** has been updated to **{formattedTime}**. We look forward to seeing you there!";
    }
}
