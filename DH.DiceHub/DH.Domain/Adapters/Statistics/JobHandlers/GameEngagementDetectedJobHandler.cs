using DH.Domain.Adapters.Statistics.Services;
using static DH.Domain.Adapters.Statistics.StatisticJobQueue;

namespace DH.Domain.Adapters.Statistics.JobHandlers;

public class GameEngagementDetectedJobHandler : IStatisticJob
{
    public Guid JobId => this.job.JobId;

    readonly GameEngagementDetectedJob job;
    readonly IStatisticsService statisticsService;

    public GameEngagementDetectedJobHandler(GameEngagementDetectedJob job, IStatisticsService statisticsService)
    {
        this.job = job;
        this.statisticsService = statisticsService;
    }

    public async Task<bool> ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            await this.statisticsService.GameEngagementDetectedMessage(this.job);
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }
}
