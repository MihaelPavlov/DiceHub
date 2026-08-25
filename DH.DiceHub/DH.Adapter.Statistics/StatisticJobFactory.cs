using DH.Domain.Adapters.Statistics;
using DH.Domain.Adapters.Statistics.JobHandlers;
using DH.Domain.Adapters.Statistics.Services;

namespace DH.Adapter.Statistics;

public class StatisticJobFactory : IStatisticJobFactory
{
    readonly IStatisticsService service;
    public StatisticJobFactory(
        IStatisticsService service)
    {
        this.service = service;
    }

    public IStatisticJob CreateHandler(IStatisticJobInfo jobInfo)
    {
        return jobInfo switch
        {
            ClubActivityDetectedJob clubJob => new ClubActivityDetectedJobHandler(clubJob, service),
            ChallengeProcessingOutcomeJob outcomeJob => new ChallengeProcessingOutcomeJobHandler(outcomeJob, service),
            EventAttendanceDetectedJob eventJob => new EventAttendanceDetectedJobHandler(eventJob, service),
            ReservationProcessingOutcomeJob reservationJob => new ReservationProcessingOutcomeJobHandler(reservationJob, service),
            RewardActionDetectedJob rewardJob => new RewardActionDetectedJobHandler(rewardJob, service),
            GameEngagementDetectedJob gameEngagementJob => new GameEngagementDetectedJobHandler(gameEngagementJob, service),
            _ => throw new NotSupportedException($"Unknown job info type: {jobInfo.GetType().Name}")
        };
    }
}
