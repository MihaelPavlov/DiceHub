using DH.Domain.Enums;

namespace DH.Domain.Entities;

public class QueuedJob
{
    public int Id { get; set; }
    public string QueueType { get; set; } = string.Empty;
    public string JobId { get; set; } = string.Empty;
    public string MessagePayload { get; set; } = string.Empty;
    public JobStatus Status { get; set; } = JobStatus.Pending;
    public string JobType { get; set; } = string.Empty;
    public DateTime EnqueuedAt { get; set; } = DateTime.UtcNow;
}
