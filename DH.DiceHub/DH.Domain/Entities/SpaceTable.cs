namespace DH.Domain.Entities;

public class SpaceTable : TenantEntity
{
    public int Id { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int MaxPeople { get; set; }
    public bool IsLocked { get; set; }
    public string Password { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public bool IsTableActive { get; set; }
    public bool IsSoloModeActive { get; set; }
    public int GameId { get; set; }

    public virtual Game Game { get; set; } = null!;
    public virtual ICollection<SpaceTableParticipant> SpaceTableParticipants { get; set; } = [];
}
