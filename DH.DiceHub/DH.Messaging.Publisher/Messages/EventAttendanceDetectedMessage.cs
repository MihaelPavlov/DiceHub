namespace DH.Messaging.Publisher.Messages;

public class EventAttendanceDetectedMessage
{
    public int EventId { get; set; }
    public DateTime? LogDate { get; set; }
    public AttendanceAction Type { get; set; }
}

public enum AttendanceAction
{
    Joining = 0,
    Leaving = 1
}