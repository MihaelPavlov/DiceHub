using DH.Domain.Enums;

namespace DH.Domain.Entities;

public class ChallengeHistoryLog
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int ChallengeId { get; set; }
    public ChallengeOutcome Outcome { get; set; }
    public DateTime OutcomeDate { get; set; }
    public DateTime CreatedDate { get; set; }
}
