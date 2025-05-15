using DH.Domain.Adapters.Statistics.Services;
using static DH.Domain.Adapters.Statistics.StatisticJobQueue;

namespace DH.Domain.Adapters.Statistics.JobHandlers;

public class ChallengeProcessingOutcomeJobHandler : IStatisticJob
{
    readonly ChallengeProcessingOutcomeJob _job;
    readonly IStatisticsService statisticsService;

    public ChallengeProcessingOutcomeJobHandler(ChallengeProcessingOutcomeJob job)
    {
        _job = job;
    }

    public Guid JobId => _job.JobId;

    public async Task<bool> ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            await statisticsService.ClubActivityDetectedMessage(job);
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }
}