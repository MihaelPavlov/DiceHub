namespace DH.Domain.Entities;

public class EventParticipant : TenantEntity
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int EventId { get; set; }

    public virtual Event Event { get; set; } = null!;
}
