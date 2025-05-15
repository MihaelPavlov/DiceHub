using DH.Domain.Adapters.Statistics;
using static DH.Domain.Adapters.Statistics.StatisticJobQueue;

namespace DH.Application.Statistics.Jobs;

public class ChallengeOutcomeJobHandler : IStatisticJob
{
    private readonly ChallengeOutcomeJob _job;

    public ChallengeOutcomeJobHandler(ChallengeOutcomeJob job)
    {
        _job = job;
    }

    public Guid JobId => _job.JobId;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine($"[ChallengeOutcome] UserId: {_job.UserId}, Challenge: {_job.ChallengeId}, Outcome: {_job.Outcome}");
        await Task.Delay(100, cancellationToken);
    }
}