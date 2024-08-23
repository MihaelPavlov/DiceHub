namespace DH.Domain.Models.GameModels.Queries;

public class GetGameByIdQueryModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int MinAge { get; set; }
    public int MinPlayers { get; set; }
    public int MaxPlayers { get; set; }
    public int AveragePlaytime { get; set; }
    public int Likes { get; set; }
    public bool IsLiked { get; set; }
    public int ImageId { get; set; }
}
