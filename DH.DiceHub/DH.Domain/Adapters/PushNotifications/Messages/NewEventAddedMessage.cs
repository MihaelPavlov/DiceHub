using DH.Domain.Adapters.PushNotifications.Messages.Common;
using DH.Domain.Helpers;
using System.Globalization;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class NewEventAddedMessage : MessageRequest
{
    public NewEventAddedMessage(string eventName, DateTime eventDate, bool isUtcFallback)
    {
        var formattedTime = eventDate.ToString(DateValidator.DATE_TIME_FORMAT, CultureInfo.InvariantCulture);
        if (isUtcFallback)
            formattedTime += " UTC (local time unavailable)";

        Title = "New Event was Added";
        Body = $"New event **{eventName}** was created, for date - {formattedTime}. Come and enjoy the journey with us";
    }
}
