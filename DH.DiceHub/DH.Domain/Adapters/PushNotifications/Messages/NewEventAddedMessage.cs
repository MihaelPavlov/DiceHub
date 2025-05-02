using DH.Domain.Adapters.PushNotifications.Helper;
using DH.Domain.Adapters.PushNotifications.Messages.Common;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class NewEventAddedMessage : MessageRequest
{
    public NewEventAddedMessage(string eventName, DateTime eventDate)
    {

        Title = "New Event was Added";
        Body = $"New event {eventName} was created, for date - {eventDate.WrapDateTime()}. Come and enjoy the journey with us";
    }
}
