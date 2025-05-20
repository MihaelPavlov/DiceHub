using DH.Domain.Adapters.PushNotifications.Helper;
using DH.Domain.Adapters.PushNotifications.Messages.Common;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class EventReminder : MessageRequest
{
    public EventReminder(string eventName, DateTime eventDate)
    {

        Title = "Reminder: Upcoming Event";
        Body = $"Don't forget! You're registered for the event \"{eventName}\" happening on {eventDate.WrapDateTime()}. We look forward to seeing you there!";
    }
}
