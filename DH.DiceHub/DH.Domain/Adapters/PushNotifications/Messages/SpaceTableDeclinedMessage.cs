using DH.Domain.Adapters.PushNotifications.Helper;
using DH.Domain.Adapters.PushNotifications.Messages.Common;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class SpaceTableDeclinedMessage : MessageRequest
{
    public SpaceTableDeclinedMessage(int numberOfGuests, DateTime reservationDate)
    {
        Title = "Table Reservation Declined";
        Body = $"You table reservation for {numberOfGuests} {(numberOfGuests == 1 ? "person" : "people")} at {reservationDate.WrapDateTime()} is DECLINED!";
    }
}
