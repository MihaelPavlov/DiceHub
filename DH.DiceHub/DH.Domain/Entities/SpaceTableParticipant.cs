namespace DH.Domain.Entities;

public class SpaceTableParticipant : TenantEntity
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int SpaceTableId { get; set; }
    public DateTime JoinedAt { get; set; }
    public bool IsVirtualParticipant { get; set; }

    public virtual SpaceTable SpaceTable { get; set; } = null!;
}
