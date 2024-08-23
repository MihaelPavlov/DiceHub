namespace DH.Domain.Entities;

public class Game
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int MinAge { get; set; }
    public int MinPlayers { get; set; }
    public int MaxPlayers { get; set; }
    public int AveragePlaytime { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
    public int CategoryId { get; set; }
    public virtual GameCategory Category { get; set; } = null!;
    public int ImageId { get; set; }
    public virtual GameImage Image { get; set; } = null!;

    public virtual ICollection<GameReview> Reviews { get; set; } = [];
    public virtual ICollection<GameLike> Likes { get; set; } = [];

    public int CopyCount { get; set; } = 1;
}
