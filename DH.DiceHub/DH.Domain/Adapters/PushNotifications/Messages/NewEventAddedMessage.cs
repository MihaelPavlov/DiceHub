using DH.Domain.Adapters.PushNotifications.Messages.Common;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class NewEventAddedMessage : MessageRequest
{
    public NewEventAddedMessage(string eventName, DateTime eventDate)
    {

        //TODO: Fix other notification dates
        Title = "New Event was Added";
        Body = $"New event {eventName} was created, for date - {eventDate.ToString("dd/MMM/yyyy HH:mm")}. Come and enjoy the journey with us";
    }
}
