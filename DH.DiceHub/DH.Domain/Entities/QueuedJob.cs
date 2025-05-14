using DH.Domain.Enums;

namespace DH.Domain.Entities;

public class QueuedJob
{
    public int Id { get; set; }
    public string QueueType { get; set; } = string.Empty;
    public Guid JobId { get; set; }
    public string MessagePayload { get; set; } = string.Empty;
    public JobStatus Status { get; set; } = JobStatus.Pending;
    public string JobType { get; set; } = string.Empty;
    public DateTime EnqueuedAt { get; set; } = DateTime.UtcNow;
}
