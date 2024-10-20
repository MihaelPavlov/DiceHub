namespace DH.Domain.Models.RewardModels.Queries;

public class GetUserRewardListQueryModel
{
    public int Id { get; set; }
    public int AvailableMoreForDays { get; set; }
    public int RewardImageId { get; set; }
    public string RewardName { get; set; } = string.Empty;
    public string RewardDescription { get; set; } = string.Empty;

    public UserRewardStatus Status { get;set; }
}

public enum UserRewardStatus
{
    Used = 0,
    Expired = 1,
    NotExpired = 2,
}