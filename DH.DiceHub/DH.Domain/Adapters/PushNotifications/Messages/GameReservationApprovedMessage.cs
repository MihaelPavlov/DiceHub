using DH.Domain.Adapters.PushNotifications.Messages.Common;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class GameReservationApprovedMessage : MessageRequest
{
    public GameReservationApprovedMessage(int numberOfGuests, string gameName, DateTime reservationDate)
    {
        Title = "Game Reservation Approved";
        Body = $"Your reservation for game {gameName} and {numberOfGuests} {(numberOfGuests == 1 ? "person" : "people")} at {reservationDate.ToShortTimeString()} is COFIRMED!";
    }
}
