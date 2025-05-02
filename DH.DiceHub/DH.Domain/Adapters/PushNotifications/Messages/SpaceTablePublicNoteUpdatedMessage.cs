using DH.Domain.Adapters.PushNotifications.Helper;
using DH.Domain.Adapters.PushNotifications.Messages.Common;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class SpaceTablePublicNoteUpdatedMessage : MessageRequest
{
    public SpaceTablePublicNoteUpdatedMessage(int numberOfGuests, DateTime reservationDate)
    {
        Title = "Table Reservation Note Changed!";
        Body = $"Your table reservation for {numberOfGuests} {(numberOfGuests == 1 ? "person" : "people")} at {reservationDate.WrapDateTime()} have a new note, please review it!";
    }
}
