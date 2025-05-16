namespace DH.Domain.Models.StatisticsModels.Queries;

public class GetChallengeHistoryLogQueryResponse
{
    public string UserId { get; set; } = string.Empty;
    public int ChallengeCount { get; set; }
}
