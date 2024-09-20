namespace DH.Domain.Entities;

public class GameQrCode
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public int GameId { get; set; }

    public virtual Game Game { get; set; } = null!;
}
