namespace DH.Domain.Entities;

public class GameReview : TenantEntity
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int GameId { get; set; }
    public string Review { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }

    public virtual Game Game { get; set; } = null!;
}
