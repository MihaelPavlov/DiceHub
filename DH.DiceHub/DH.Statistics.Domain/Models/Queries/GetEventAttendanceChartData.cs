namespace DH.Statistics.Domain.Models.Queries;

public class GetEventAttendanceChartData
{
    public List<EventAttendance> EventAttendances { get; set; } = new();
}

public class EventAttendance
{
    public int UserAttendedCount { get; set; }
    public int EventId { get; set; }
}