namespace DH.Domain.Entities;

public class SpaceTableParticipant
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int SpaceTableId { get; set; }

    public virtual SpaceTable SpaceTable { get; set; } = null!;
}
