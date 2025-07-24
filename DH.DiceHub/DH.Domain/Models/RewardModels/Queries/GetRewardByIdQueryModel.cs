using DH.Domain.Enums;

namespace DH.Domain.Models.RewardModels.Queries;

public class GetRewardByIdQueryModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal CashEquivalent { get; set; }
    public string Description { get; set; } = string.Empty;
    public RewardRequiredPoint RequiredPoints { get; set; }
    public RewardLevel Level { get; set; }
    public int ImageId { get; set; }
}
