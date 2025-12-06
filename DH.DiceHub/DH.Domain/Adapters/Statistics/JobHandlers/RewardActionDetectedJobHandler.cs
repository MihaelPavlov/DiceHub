using DH.Domain.Adapters.Statistics.Services;

namespace DH.Domain.Adapters.Statistics.JobHandlers;

public class RewardActionDetectedJobHandler : IStatisticJob
{
    public string JobId => this.job.JobId;

    readonly RewardActionDetectedJob job;
    readonly IStatisticsService statisticsService;

    public RewardActionDetectedJobHandler(RewardActionDetectedJob job, IStatisticsService statisticsService)
    {
        this.job = job;
        this.statisticsService = statisticsService;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await this.statisticsService.RewardActionDetectedMessage(this.job);
    }
}
