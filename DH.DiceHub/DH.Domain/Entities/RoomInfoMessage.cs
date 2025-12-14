namespace DH.Domain.Entities;

public class RoomInfoMessage : TenantEntity
{
    public int Id { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string MessageContentKey { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public int RoomId { get; set; }

    public virtual Room Room { get; set; } = null!;
}
