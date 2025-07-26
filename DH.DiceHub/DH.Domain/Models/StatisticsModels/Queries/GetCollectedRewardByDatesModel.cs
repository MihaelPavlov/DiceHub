namespace DH.Domain.Models.StatisticsModels.Queries;

public class GetCollectedRewardByDatesModel
{
    public int RewardId { get; set; }
    public int CollectedCount { get; set; }
    public decimal TotalCashEquivalent { get; set; }
}
