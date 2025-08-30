namespace DH.Domain.Models.RewardModels.Queries;

public class GetSystemRewardListQueryModel
{
    public int Id { get; set; }
    public string Name_EN { get; set; } = string.Empty;
    public string Name_BG { get; set; } = string.Empty;
    public decimal CashEquivalent { get; set; }
    public string Description_EN { get; set; } = string.Empty;
    public string Description_BG { get; set; } = string.Empty;
    public int ImageId { get; set; }
}
