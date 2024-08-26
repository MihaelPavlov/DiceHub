namespace DH.Domain.Entities;

public class GameImage
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public byte[] Data { get; set; } = [];
    public int GameId { get; set; }

    public virtual Game Game { get; set; } = null!;
}
