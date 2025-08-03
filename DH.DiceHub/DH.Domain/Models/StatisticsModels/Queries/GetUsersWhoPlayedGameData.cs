namespace DH.Domain.Models.StatisticsModels.Queries;

public class GetUsersWhoPlayedGameData
{
    public List<GameUserActivity> Users { get; set; } = new();
}

public class GameUserActivity
{
    public string UserId { get; set; } = string.Empty;
    public string UserDisplayName { get; set; } = string.Empty;
    public DateTime PlayedAt { get; set; }
    public int TimesPlayedFromUser { get; set; }
}