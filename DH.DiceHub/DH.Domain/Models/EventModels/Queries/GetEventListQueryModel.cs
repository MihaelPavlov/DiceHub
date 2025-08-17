namespace DH.Domain.Models.EventModels.Queries;

public class GetEventListQueryModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description_EN { get; set; } = string.Empty;
    public string Description_BG { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public int PeopleJoined { get; set; }
    public int MaxPeople { get; set; }
    public int GameId { get; set; }
    public bool IsCustomImage { get; set; }
    public int ImageId { get; set; }
}
