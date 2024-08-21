﻿namespace DH.Domain.Entities;

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
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public int CategoryId { get; set; }
    public virtual GameCategory Category { get; set; } = null!;

    public virtual ICollection<GameReview> Reviews { get; set; } = [];
    public virtual ICollection<GameLike> Likes { get; set; } = [];
}
