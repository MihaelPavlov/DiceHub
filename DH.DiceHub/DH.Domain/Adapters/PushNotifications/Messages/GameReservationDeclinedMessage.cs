using DH.Domain.Adapters.PushNotifications.Messages.Common;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class GameReservationDeclinedMessage : MessageRequest
{
    public GameReservationDeclinedMessage(int numberOfGuests, string gameName, DateTime reservationDate)
    {
        Title = "Game Reservation DECLINED";
        Body = $"Your reservation for game {gameName} and {numberOfGuests} {(numberOfGuests == 1 ? "person" : "people")} at {reservationDate.ToShortTimeString()} is DECLINED!";
    }
}
