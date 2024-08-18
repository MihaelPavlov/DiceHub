namespace DH.Domain.Entities;

public class Game
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string MinAge { get; set; } = string.Empty;
    public int MinPlayers { get; set; }
    public int MaxPlayers { get; set; }
    public int AveragePlaytime { get; set; }
    public string ImageUrl { get; set; } = string.Empty;

    public virtual ICollection<GameReview> Reviews { get; set; } = [];
    public virtual ICollection<GameLike> Likes { get; set; } = [];
}
