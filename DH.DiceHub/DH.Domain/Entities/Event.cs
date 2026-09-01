namespace DH.Domain.Entities;

public class Event : TenantEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description_EN { get; set; } = string.Empty;
    public string Description_BG { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public int MaxPeople { get; set; }
    public int GameId { get; set; }
    public bool IsCustomImage { get; set; } = false;
    public bool IsDeleted { get; set; }
    public bool IsJoinChallengeProcessed { get; set; }
    public string ImageUrl { get; set; } = string.Empty;

    public virtual Game Game { get; set; } = null!;
    public virtual ICollection<EventParticipant> Participants { get; set; } = [];
    public virtual ICollection<EventNotification> Notifications { get; set; } = [];
}
