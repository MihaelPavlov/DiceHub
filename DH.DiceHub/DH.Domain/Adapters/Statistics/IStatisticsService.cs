namespace DH.Domain.Adapters.Statistics;

public interface IStatisticsService
{
    Task ClubActivityDetectedMessage(string? userId = null);

    Task EventAttendanceDetectedMessage(string action, int eventId);

    Task ReservationProcessingOutcomeMessage(string action, string userId, string type, int reservationId);

    Task RewardActionDetectedMessage(string userId, int rewardId, bool isExpired, bool isCollected);

    Task ChallengeProcessingOutcomeMessage(string userId, int challengeId, string type);
}
