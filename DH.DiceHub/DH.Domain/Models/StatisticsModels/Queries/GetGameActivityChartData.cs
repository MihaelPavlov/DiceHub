namespace DH.Domain.Models.StatisticsModels.Queries;

public class GetGameActivityChartData
{
    public List<GameActivityStats> Games { get; set; } = new();
}

public class GameActivityStats
{
    public int GameId { get; set; }
    public string GameName { get; set; } = string.Empty;
    public int TimesPlayed { get; set; }
    public int GameImageId { get; set; }
}
