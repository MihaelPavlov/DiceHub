using DH.Domain.Enums;

namespace DH.Domain.Entities;

public class Game
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;
    public string Description_EN { get; set; } = string.Empty;
    public string Description_BG { get; set; } = string.Empty;
    public int MinAge { get; set; }
    public int MinPlayers { get; set; }
    public int MaxPlayers { get; set; }
    public GameAveragePlaytime AveragePlaytime { get; set; }
    public int CategoryId { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
    public string ImageUrl { get; set; } = string.Empty;

    public virtual GameCategory Category { get; set; } = null!;
    public virtual GameInventory Inventory { get; set; } = null!;

    public virtual ICollection<GameReview> Reviews { get; set; } = [];
    public virtual ICollection<GameLike> Likes { get; set; } = [];
    public virtual ICollection<GameReservation> Reservations { get; set; } = [];
    public virtual ICollection<Room> Rooms { get; set; } = [];
}
