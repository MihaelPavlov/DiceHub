using DH.Application.Statistics.Jobs;
using DH.Domain.Adapters.Statistics;
using static DH.Domain.Adapters.Statistics.StatisticJobQueue;

namespace DH.Application.Statistics;

public class StatisticJobFactory : IStatisticJobFactory
{
    public IStatisticJob CreateHandler(IStatisticJobInfo jobInfo)
    {
        return jobInfo switch
        {
            ClubActivityDetectedJob clubJob => new ClubActivityDetectedJobHandler(clubJob),
            ChallengeOutcomeJob outcomeJob => new ChallengeOutcomeJobHandler(outcomeJob),
            _ => throw new NotSupportedException($"Unknown job info type: {jobInfo.GetType().Name}")
        };
    }
}
