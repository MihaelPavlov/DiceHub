namespace DH.Domain.Entities;

public class RoomMessage
{
    public int Id { get; set; }
    public string Sender { get; set; } = string.Empty;
    public string MessageContent { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public int RoomId { get; set; }

    public virtual Room Room { get; set; } = null!;
}
