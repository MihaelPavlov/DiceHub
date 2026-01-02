namespace DH.Domain.Models.RewardModels.Queries;

public class GetUserRewardListQueryModel
{
    public int Id { get; set; }
    public int AvailableMoreForDays { get; set; }
    public string RewardImageUrl { get; set; } = string.Empty;
    public string RewardName_EN { get; set; } = string.Empty;
    public string RewardName_BG { get; set; } = string.Empty;
    public string RewardDescription_EN { get; set; } = string.Empty;
    public string RewardDescription_BG { get; set; } = string.Empty;

    public UserRewardStatus Status { get; set; }
}

public enum UserRewardStatus
{
    Used = 0,
    Expired = 1,
    NotExpired = 2,
}