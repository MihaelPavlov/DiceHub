namespace DH.Domain.Entities;

public class Event
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public int PeopleJoined { get; set; }
    public int MaxPeople { get; set; }
    public int GameId { get; set; }
    public bool IsCustomImage { get; set; } = false;

    public virtual Game Game { get; set; } = null!;
    public virtual EventImage? Image { get; set; }
}
