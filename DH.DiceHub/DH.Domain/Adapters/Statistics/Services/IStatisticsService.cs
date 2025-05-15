using static DH.Domain.Adapters.Statistics.StatisticJobQueue;

namespace DH.Domain.Adapters.Statistics.Services;

public interface IStatisticsService
{
    Task ClubActivityDetectedMessage(ClubActivityDetectedJob job);

    Task EventAttendanceDetectedMessage(EventAttendanceDetectedJob job);

    Task ReservationProcessingOutcomeMessage(ReservationProcessingOutcomeJob job);

    Task RewardActionDetectedMessage(RewardActionDetectedJob job);

    Task ChallengeProcessingOutcomeMessage(ChallengeProcessingOutcomeJob job);
}
