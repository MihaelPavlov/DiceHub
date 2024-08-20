namespace DH.Domain.Entities;

public class GameCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public virtual ICollection<Game> Games { get; set; } = [];
}
