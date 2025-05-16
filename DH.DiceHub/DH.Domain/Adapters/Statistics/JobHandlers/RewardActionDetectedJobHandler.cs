using DH.Domain.Adapters.Statistics.Services;
using static DH.Domain.Adapters.Statistics.StatisticJobQueue;

namespace DH.Domain.Adapters.Statistics.JobHandlers;

public class RewardActionDetectedJobHandler : IStatisticJob
{
    public Guid JobId => this.job.JobId;

    readonly RewardActionDetectedJob job;
    readonly IStatisticsService statisticsService;

    public RewardActionDetectedJobHandler(RewardActionDetectedJob job, IStatisticsService statisticsService)
    {
        this.job = job;
        this.statisticsService = statisticsService;
    }

    public async Task<bool> ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            await this.statisticsService.RewardActionDetectedMessage(this.job);
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }
}
