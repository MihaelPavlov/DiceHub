using DH.Domain.Adapters.PushNotifications.Messages.Common;
using DH.Domain.Helpers;
using System.Globalization;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class GameReservationManagementReminder : MessageRequest
{
    public string GameName { get; set; }
    public string Email { get; set; }
    public int CountPeople { get; set; }
    public DateTime ReservationTime { get; set; }

    public GameReservationManagementReminder(
        string gameName, string userEmail,
        int countPeople, DateTime reservationTime,
        bool isUtcFallback)
    {
        this.GameName = gameName;
        this.ReservationTime = reservationTime;
        this.Email = userEmail;
        this.CountPeople = countPeople;

        var formattedTime = reservationTime.ToString(DateValidator.DATE_TIME_FORMAT, CultureInfo.InvariantCulture);
        if (isUtcFallback)
            formattedTime += " UTC (local time unavailable)";

        Title = "New Game Reservation Reminder";
        Body = $"User {this.Email} made a new reservation. Reserve a table for {this.CountPeople} people to play **{this.GameName}** at {formattedTime}.";
    }
}