using DH.Domain.Adapters.PushNotifications.Messages.Common;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class GameReservationReminder : MessageRequest
{
    public string GameName { get; set; }
    public DateTime ReservationTime { get; set; }

    public GameReservationReminder(string gameName, DateTime reservationTime)
    {
        this.GameName = gameName;
        this.ReservationTime = reservationTime;

        Title = "Game Reservation Reminder";
        Body = $"You have reserved a table for {this.GameName} at {this.ReservationTime.ToShortTimeString()}!";
    }
}