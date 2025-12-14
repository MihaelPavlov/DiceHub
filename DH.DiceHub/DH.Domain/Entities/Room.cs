namespace DH.Domain.Entities;

public class Room : TenantEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime StartDate { get; set; }
    public int MaxParticipants { get; set; }
    public int GameId { get; set; }
    public string UserId { get; set; } = string.Empty;

    public virtual Game Game { get; set; } = null!;
    public virtual ICollection<RoomParticipant> Participants { get; set; } = [];
    public virtual ICollection<RoomMessage> Messages { get; set; } = [];
}
