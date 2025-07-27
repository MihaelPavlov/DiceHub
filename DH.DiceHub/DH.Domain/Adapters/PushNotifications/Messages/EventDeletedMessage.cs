using DH.Domain.Adapters.PushNotifications.Messages.Common;
using DH.Domain.Helpers;
using System.Globalization;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class EventDeletedMessage : MessageRequest
{
    public EventDeletedMessage(string eventName, DateTime eventDate, bool isUtcFallback)
    {
        var formattedTime = eventDate.ToString(DateValidator.DATE_TIME_FORMAT, CultureInfo.InvariantCulture);
        if (isUtcFallback)
            formattedTime += " UTC (local time unavailable)";

        Title = "Notice: Event Cancelled";
        Body = $"We're sorry to inform you that the event **{eventName}**, originally scheduled for **{formattedTime}**, has been cancelled. We apologize for the inconvenience and appreciate your understanding.";
    }
}