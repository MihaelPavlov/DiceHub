using DH.Domain.Adapters.PushNotifications.Helper;
using DH.Domain.Adapters.PushNotifications.Messages.Common;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class GameReservationManagementReminder : MessageRequest
{
    public string GameName { get; set; }
    public string Email { get; set; }
    public int CountPeople { get; set; }
    public DateTime ReservationTime { get; set; }

    public GameReservationManagementReminder(string gameName, string userEmail, int countPeople, DateTime reservationTime)
    {
        this.GameName = gameName;
        this.ReservationTime = reservationTime;
        this.Email = userEmail;
        this.CountPeople = countPeople;

        Title = "New Game Reservation Reminder";
        Body = $"You are having a new reservation user-email {this.Email}. Reserve a table for {this.CountPeople} people and game {this.GameName} at {this.ReservationTime.WrapDateTime()}!";
    }
}