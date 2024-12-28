namespace DH.Statistics.Domain.Entities;

public class EventAttendanceLog
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime LogDate { get; set; }
    public DateTime CreatedDate { get; set; }
}
