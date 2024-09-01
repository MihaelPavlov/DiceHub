namespace DH.Domain.Entities;

public class Room
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime StartDate { get; set; }
    public int MaxParticipants { get; set; }
    public int GameId { get; set; }

    public virtual Game Game { get; set; } = null!;
}
