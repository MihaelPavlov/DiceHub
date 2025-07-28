namespace DH.Domain.Adapters.Scheduling.Models;

public class ScheduleJobInfo
{
    public string JobKeyName { get; set; } = string.Empty;
    public string TriggerKeyName { get; set; } = string.Empty;
    public DateTime? NextFireTime { get; set; }
    public DateTime? PreviousFireTime { get; set; }
}
