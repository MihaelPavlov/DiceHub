using DH.Domain.Adapters.Statistics.Services;

namespace DH.Domain.Adapters.Statistics.JobHandlers;

public class ReservationProcessingOutcomeJobHandler : IStatisticJob
{
    public string JobId => this.job.JobId;

    readonly ReservationProcessingOutcomeJob job;
    readonly IStatisticsService statisticsService;

    public ReservationProcessingOutcomeJobHandler(ReservationProcessingOutcomeJob job, IStatisticsService statisticsService)
    {
        this.job = job;
        this.statisticsService = statisticsService;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await this.statisticsService.ReservationProcessingOutcomeMessage(this.job);
    }
}
