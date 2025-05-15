namespace DH.Domain.Models.StatisticsModels.Queries;

public class GetExpiredCollectedRewardsChartDataModel
{
    public List<RewardsStats> Expired { get; set; } = new();
    public List<RewardsStats> Collected { get; set; } = new();
}

public class RewardsStats
{
    public int Month { get; set; }
    public int CountRewards { get; set; }
}
