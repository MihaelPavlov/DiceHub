using DH.Domain.Adapters.PushNotifications.Helper;
using DH.Domain.Adapters.PushNotifications.Messages.Common;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class GameReservationReminderForUser : MessageRequest
{
    public string GameName { get; set; }
    public DateTime ReservationTime { get; set; }

    public GameReservationReminderForUser(string gameName, DateTime reservationTime)
    {
        this.GameName = gameName;
        this.ReservationTime = reservationTime;

        Title = "Game Reservation Reminder";
        Body = $"You have reserved a table for {this.GameName} at {this.ReservationTime.WrapDateTime()}!";
    }
}