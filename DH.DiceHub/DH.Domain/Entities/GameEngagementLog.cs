namespace DH.Domain.Entities;

public class GameEngagementLog
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime DetectedOn { get; set; }
    public DateTime CreatedDate { get; set; }
}
