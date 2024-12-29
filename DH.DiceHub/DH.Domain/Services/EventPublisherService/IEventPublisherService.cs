namespace DH.Domain.Services.Publisher;

public interface IEventPublisherService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task PublishClubActivityDetectedMessage(string? userId = null);

    /// <summary>
    /// Publishing event for creating an event log record
    /// </summary>
    /// <param name="action">Actions is representing AttendanceType -> Joining | Leaving. </param>
    /// <param name="eventId"></param>
    /// <returns></returns>
    Task PublishEventAttendanceDetectedMessage(string action, int eventId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="action">Actions is representing ReservationOutcome -> Completed | Cancelled. </param>
    /// <param name="type">Actions is representing ReservationType -> Game | Table. </param>
    /// <param name="reservationId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task PublishReservationProcessingOutcomeMessage(string action, string userId, string type, int reservationId);
}
