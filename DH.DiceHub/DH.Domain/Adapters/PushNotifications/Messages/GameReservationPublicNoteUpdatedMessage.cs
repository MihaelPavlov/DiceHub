using DH.Domain.Adapters.PushNotifications.Messages.Common;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class GameReservationPublicNoteUpdatedMessage : MessageRequest
{
    public GameReservationPublicNoteUpdatedMessage(int numberOfGuests, DateTime reservationDate)
    {
        Title = "Game Reservation Note Changed!";
        Body = $"Your game reservation for {numberOfGuests} {(numberOfGuests == 1 ? "person" : "people")} at {reservationDate.ToShortTimeString()} have a new note, please review it!";
    }
}