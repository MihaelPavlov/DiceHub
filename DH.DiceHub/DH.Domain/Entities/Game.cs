namespace DH.Domain.Entities;

public class Game
{
    //public Game() { }
    public Game(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; set; }
    public string Name { get; set; }
}
