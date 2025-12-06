using DH.Domain.Adapters.Statistics.Services;

namespace DH.Domain.Adapters.Statistics.JobHandlers;

public class GameEngagementDetectedJobHandler : IStatisticJob
{
    public string JobId => this.job.JobId;

    readonly GameEngagementDetectedJob job;
    readonly IStatisticsService statisticsService;

    public GameEngagementDetectedJobHandler(GameEngagementDetectedJob job, IStatisticsService statisticsService)
    {
        this.job = job;
        this.statisticsService = statisticsService;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await this.statisticsService.GameEngagementDetectedMessage(this.job);
    }
}
