using DH.Domain.Adapters.PushNotifications.Helper;
using DH.Domain.Adapters.PushNotifications.Messages.Common;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class EventDeletedMessage : MessageRequest
{
    public EventDeletedMessage(string eventName, DateTime eventDate)
    {
        Title = "Notice: Event Cancelled";
        Body = $"The event \"{eventName}\" scheduled for {eventDate.WrapDateTime()} has been cancelled. We apologize for the inconvenience and appreciate your understanding.";
    }
}