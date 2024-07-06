namespace DH.Domain.Models.GameModels.Queries;

public class GetGameReviewListQueryModel
{
    public int GameId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserImageUrl { get; set; } = string.Empty;
    public string UserFullName { get; set; } = string.Empty;
    public string Review { get; set; } = string.Empty;
}