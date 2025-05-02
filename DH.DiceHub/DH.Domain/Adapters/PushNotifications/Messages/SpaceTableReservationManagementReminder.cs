using DH.Domain.Adapters.PushNotifications.Helper;
using DH.Domain.Adapters.PushNotifications.Messages.Common;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class SpaceTableReservationManagementReminder : MessageRequest
{
    public SpaceTableReservationManagementReminder(int countPeople, DateTime reservationTime)
    {
        Title = "New Table Reservation Reminder";
        Body = $"You are having a new table reservation. Reserve a table for {countPeople} {(countPeople == 1 ? "person" : "people")} at {reservationTime.WrapDateTime()}!";
    }
}
