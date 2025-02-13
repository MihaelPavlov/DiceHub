using DH.Domain.Enums;

namespace DH.Domain.Models.ChallengeModels.Queries;

public class GetUserChallengePeriodPerformanceQueryModel
{
    public int Id { get; set; }
    public int Points { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public TimePeriodType TimePeriodType { get; set; }
}
