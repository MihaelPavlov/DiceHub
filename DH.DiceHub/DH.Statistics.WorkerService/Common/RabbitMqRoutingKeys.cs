namespace DH.Statistics.WorkerService.Common;

public class RabbitMqRoutingKeys
{
    public string ClubActivityDetected { get; set; } = string.Empty;
    public string EventAttendanceDetected { get; set; } = string.Empty;
    public string ReservationProcessingOutcome { get; set; } = string.Empty;
    public string ChallengeProcessingOutcome { get; set; } = string.Empty;
    public string RewardActionDetected { get; set; } = string.Empty;
}
