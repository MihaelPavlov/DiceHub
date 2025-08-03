using DH.Domain.Adapters.Statistics;
using DH.Domain.Adapters.Statistics.JobHandlers;
using DH.Domain.Adapters.Statistics.Services;
using Microsoft.Extensions.DependencyInjection;
using static DH.Domain.Adapters.Statistics.StatisticJobQueue;

namespace DH.Adapter.Statistics;

public class StatisticJobFactory : IStatisticJobFactory
{
    readonly IServiceScopeFactory serviceScopeFactory;
    public StatisticJobFactory(IServiceScopeFactory serviceScopeFactory)
    {
        this.serviceScopeFactory = serviceScopeFactory;
    }

    public IStatisticJob CreateHandler(IStatisticJobInfo jobInfo)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IStatisticsService>();

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
