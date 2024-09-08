namespace DH.Domain.Entities;

public class RoomParticipant
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }
    public bool IsDeleted { get; set; }
    public int RoomId { get; set; }

    public virtual Room Room { get; set; } = null!;
}
