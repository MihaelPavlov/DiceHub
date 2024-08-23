namespace DH.Domain.Models.GameModels.Queries;

public class GetGameListQueryModel
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Likes { get; set; }
    public bool IsLiked { get; set; }
    public int ImageId { get; set; }
}
