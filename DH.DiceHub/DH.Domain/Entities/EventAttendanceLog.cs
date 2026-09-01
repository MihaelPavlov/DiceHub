namespace DH.Domain.Entities;

public class EventAttendanceLog : TenantEntity
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int EventId { get; set; }
    public DateTime LogDate { get; set; }
    public DateTime CreatedDate { get; set; }
}
