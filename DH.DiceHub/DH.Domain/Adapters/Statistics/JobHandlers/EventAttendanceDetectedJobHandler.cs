using DH.Domain.Adapters.Statistics.Services;

namespace DH.Domain.Adapters.Statistics.JobHandlers;

public class EventAttendanceDetectedJobHandler : IStatisticJob
{
    public string JobId => this.job.JobId;

    readonly EventAttendanceDetectedJob job;
    readonly IStatisticsService statisticsService;

    public EventAttendanceDetectedJobHandler(EventAttendanceDetectedJob job, IStatisticsService statisticsService)
    {
        this.job = job;
        this.statisticsService = statisticsService;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await this.statisticsService.EventAttendanceDetectedMessage(this.job);
    }
}
