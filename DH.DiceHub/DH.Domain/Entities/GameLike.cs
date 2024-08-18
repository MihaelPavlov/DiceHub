namespace DH.Domain.Entities;

public class GameLike
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int GameId { get; set; }
    public virtual Game Game { get; set; } = null!;
}
