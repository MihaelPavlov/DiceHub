using DH.Domain.Adapters.PushNotifications.Messages.Common;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class SpaceTablePublicNoteUpdatedMessage : MessageRequest
{
    public SpaceTablePublicNoteUpdatedMessage(int numberOfGuests, DateTime reservationDate)
    {
        Title = "Table Reservation Note Changed!";
        Body = $"Your table reservation for {numberOfGuests} {(numberOfGuests == 1 ? "person" : "people")} at {reservationDate.ToShortTimeString()} have a new note, please review it!";
    }
}
