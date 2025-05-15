using DH.Domain.Adapters.Statistics;
using static DH.Domain.Adapters.Statistics.StatisticJobQueue;

namespace DH.Application.Statistics.Jobs;

public record ClubActivityDetectedJobHandler : IStatisticJob
{
    private readonly ClubActivityDetectedJob _job;

    public ClubActivityDetectedJobHandler(ClubActivityDetectedJob job)
    {
        _job = job;
    }
    public Guid JobId => _job.JobId;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine($"[ClubActivity] UserId: {_job.UserId}, LogDate: {_job.LogDate}");
        await Task.Delay(100, cancellationToken);
    }
}
