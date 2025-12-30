using DH.Domain.Enums;

namespace DH.Domain.Models.EventModels.Queries;

public class GetEventByIdQueryModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description_EN { get; set; } = string.Empty;
    public string Description_BG { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public int PeopleJoined { get; set; }
    public int MaxPeople { get; set; }
    public bool IsCustomImage { get; set; }
    public string ImageUrl { get; set; } = string.Empty;

    public int GameId { get; set; }
    public string GameName { get; set; } = string.Empty;
    public string GameDescription_EN { get; set; } = string.Empty;
    public string GameDescription_BG { get; set; } = string.Empty;
    public int GameMinAge { get; set; }
    public int GameMinPlayers { get; set; }
    public int GameMaxPlayers { get; set; }
    public GameAveragePlaytime GameAveragePlaytime { get; set; }
}
