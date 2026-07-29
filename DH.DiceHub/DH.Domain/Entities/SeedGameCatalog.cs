using DH.Domain.Enums;

namespace DH.Domain.Entities;

public class SeedGameCatalog
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description_EN { get; set; } = string.Empty;
    public string Description_BG { get; set; } = string.Empty;
    public int MinAge { get; set; }
    public int MinPlayers { get; set; }
    public int MaxPlayers { get; set; }
    public GameAveragePlaytime AveragePlaytime { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string ImageFileName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
