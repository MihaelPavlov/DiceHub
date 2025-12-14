namespace DH.Domain.Entities;

public class GameInventory : TenantEntity
{
    public int Id { get; set; }
    public int TotalCopies { get; set; } = 1;
    public int AvailableCopies { get; set; }
    public int GameId { get; set; }

    public virtual Game Game { get; set; } = null!;
}
