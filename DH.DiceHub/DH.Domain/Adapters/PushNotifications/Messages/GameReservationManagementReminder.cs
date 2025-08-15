using DH.Domain.Adapters.Localization;
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
        bool isUtcFallback, ILocalizationService localizer)
    {
        this.GameName = gameName;
        this.ReservationTime = reservationTime;
        this.Email = userEmail;
        this.CountPeople = countPeople;

        var formattedTime = reservationTime.ToString(DateValidator.DATE_TIME_FORMAT, CultureInfo.InvariantCulture);
        if (isUtcFallback)
            formattedTime += localizer["UtcFallbackNotice"];

        var peopleWord = CountPeople == 1 ? localizer["W_Person"] : localizer["W_People"];

        Title = localizer["NewGameReservationReminderTitle"];
        Body = string.Format(localizer["NewGameReservationReminderBody"], this.Email, this.CountPeople, peopleWord, this.GameName, formattedTime);
    }
}