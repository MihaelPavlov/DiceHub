using DH.Domain.Adapters.Statistics.Services;
using static DH.Domain.Adapters.Statistics.StatisticJobQueue;

namespace DH.Domain.Adapters.Statistics.JobHandlers;

public record ClubActivityDetectedJobHandler : IStatisticJob
{
    public Guid JobId => job.JobId;

    readonly ClubActivityDetectedJob job;
    readonly IStatisticsService statisticsService;

    public ClubActivityDetectedJobHandler(ClubActivityDetectedJob job, IStatisticsService statisticsService)
    {
        this.job = job;
        this.statisticsService = statisticsService;
    }

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
