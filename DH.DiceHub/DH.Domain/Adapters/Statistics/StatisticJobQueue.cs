using DH.Domain.Queue;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace DH.Domain.Adapters.Statistics;

public class StatisticJobQueue : QueueBase
{
    private readonly ConcurrentQueue<IStatisticJob> jobs = new();

    public override string QueueName => QueueNameKeysConstants.STATISTICS_QUEUE_NAME;

    public void Enqueue(IStatisticJob job)
    {
        jobs.Enqueue(job);
    }

    public virtual bool TryDequeue([MaybeNullWhen(false)] out IStatisticJob result)
    {
        return jobs.TryDequeue(out result);
    }

    public void RequeueClubActivityDetectedJob(IStatisticJob job)
    {
        jobs.Enqueue(job);
    }

    public record ClubActivityDetectedJob(string UserId, DateTime LogDate) : JobInfoBase, IStatisticJobInfo;
    public record ChallengeOutcomeJob(
        string UserId, int ChallengeId,
        DateTime OutcomeDate, string Outcome, DateTime LogDate) : JobInfoBase, IStatisticJobInfo;
}