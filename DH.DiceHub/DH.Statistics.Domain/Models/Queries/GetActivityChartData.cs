namespace DH.Statistics.Domain.Models.Queries;

public class GetActivityChartData
{
    public List<ActivityLog> Logs { get; set; } = new();
}

public class ActivityLog
{
    public int UserCount { get; set; }
    public DateTime Date { get; set; }
}