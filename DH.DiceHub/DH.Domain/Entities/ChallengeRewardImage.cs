namespace DH.Domain.Entities;

public class ChallengeRewardImage : TenantEntity
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public byte[] Data { get; set; } = [];
    public int RewardId { get; set; }

    public virtual ChallengeReward Reward { get; set; } = null!;
}
