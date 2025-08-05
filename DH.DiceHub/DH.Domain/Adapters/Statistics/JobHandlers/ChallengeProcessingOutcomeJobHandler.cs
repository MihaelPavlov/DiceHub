using DH.Domain.Adapters.Statistics.Services;
using static DH.Domain.Adapters.Statistics.StatisticJobQueue;

namespace DH.Domain.Adapters.Statistics.JobHandlers;

public class ChallengeProcessingOutcomeJobHandler : IStatisticJob
{
    public Guid JobId => this.job.JobId;

    readonly ChallengeProcessingOutcomeJob job;
    readonly IStatisticsService statisticsService;

    public ChallengeProcessingOutcomeJobHandler(ChallengeProcessingOutcomeJob job, IStatisticsService statisticsService)
    {
        this.job = job;
        this.statisticsService = statisticsService;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await this.statisticsService.ChallengeProcessingOutcomeMessage(this.job);
    }
}