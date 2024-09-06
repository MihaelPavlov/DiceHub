namespace DH.Domain.Entities;

public class RoomMessages
{
    public int Id { get; set; }
    public string Sender { get; set; } = string.Empty;
    public string MessageContent { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public int RoomId { get; set; }

    public virtual Room Room { get; set; } = null!;
}
