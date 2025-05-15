using DH.Domain.Adapters.Statistics.Services;
using static DH.Domain.Adapters.Statistics.StatisticJobQueue;

namespace DH.Domain.Adapters.Statistics.JobHandlers;

public class ReservationProcessingOutcomeJobHandler : IStatisticJob
{
    public Guid JobId => job.JobId;

    readonly ReservationProcessingOutcomeJob job;
    readonly IStatisticsService statisticsService;

    public ReservationProcessingOutcomeJobHandler(ReservationProcessingOutcomeJob job, IStatisticsService statisticsService)
    {
        this.job = job;
        this.statisticsService = statisticsService;
    }

    public async Task<bool> ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            await statisticsService.ReservationProcessingOutcomeMessage(job);
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }
}
