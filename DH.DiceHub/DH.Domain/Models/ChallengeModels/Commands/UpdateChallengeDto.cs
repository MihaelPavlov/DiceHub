using DH.Domain.Enums;

namespace DH.Domain.Models.ChallengeModels.Commands;

public class UpdateChallengeDto 
{
    public int Id { get; set; }
    public int RewardPoints { get; set; }
    public int Attempts { get; set; }
    public ChallengeType Type { get; set; }
    public int GameId { get; set; }
}
