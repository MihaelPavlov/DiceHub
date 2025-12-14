namespace DH.Domain.Entities;

public class EventImage : TenantEntity
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public byte[] Data { get; set; } = [];
    public int EventId { get; set; }

    public virtual Event Event { get; set; } = null!;
}
