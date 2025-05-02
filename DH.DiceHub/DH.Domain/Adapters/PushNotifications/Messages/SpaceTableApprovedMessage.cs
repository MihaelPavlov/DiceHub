using DH.Domain.Adapters.PushNotifications.Helper;
using DH.Domain.Adapters.PushNotifications.Messages.Common;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class SpaceTableApprovedMessage : MessageRequest
{
    public SpaceTableApprovedMessage(int numberOfGuests, DateTime reservationDate)
    {
        Title = "Table Reservation Approved";
        Body = $"Your table reservation for {numberOfGuests} {(numberOfGuests == 1 ? "person" : "people")} at {reservationDate.WrapDateTime()} is COFIRMED!";
    }
}
