using DH.Domain.Enums;

namespace DH.Domain.Models.RewardModels.Queries;

public class GetRewardByIdQueryModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int RequiredPoints { get; set; }
    public RewardLevel Level { get; set; }
    public int ImageId { get; set; }
}
