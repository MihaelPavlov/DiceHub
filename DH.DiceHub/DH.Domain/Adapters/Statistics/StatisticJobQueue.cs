using DH.Domain.Enums;
using DH.Domain.Queue;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace DH.Domain.Adapters.Statistics;

public class StatisticJobQueue : QueueBase
{
    private readonly ConcurrentQueue<IStatisticJobInfo> jobs = new();

    public override string QueueName => QueueNameKeysConstants.STATISTICS_QUEUE_NAME;

    public void Enqueue(IStatisticJobInfo job)
    {
        jobs.Enqueue(job);
    }

    public virtual bool TryDequeue([MaybeNullWhen(false)] out IStatisticJobInfo result)
    {
        return jobs.TryDequeue(out result);
    }

    public void RequeueClubActivityDetectedJob(IStatisticJobInfo job)
    {
        jobs.Enqueue(job);
    }

    public record ClubActivityDetectedJob(
        string UserId,
        DateTime LogDate) : JobInfoBase, IStatisticJobInfo
    {
        public StatisticJobType JobType => StatisticJobType.ClubActivityDetected;
    }

    public record ChallengeProcessingOutcomeJob(
        string UserId,
        int ChallengeId,
        ChallengeOutcome Outcome,
        DateTime OutcomeDate,
        DateTime LogDate) : JobInfoBase, IStatisticJobInfo
    {
        public StatisticJobType JobType => StatisticJobType.ChallengeProcessingOutcome;
    }

    public record EventAttendanceDetectedJob(
        string UserId,
        AttendanceAction Type,
        int EventId,
        DateTime LogDate) : JobInfoBase, IStatisticJobInfo
    {
        public StatisticJobType JobType => StatisticJobType.EventAttendanceDetected;
    }

    public record ReservationProcessingOutcomeJob(
        string UserId,
        ReservationOutcome Outcome,
        ReservationType Type,
        int ReservationId,
        DateTime OutcomeDate,
        DateTime LogDate) : JobInfoBase, IStatisticJobInfo
    {
        public StatisticJobType JobType => StatisticJobType.ReservationProcessingOutcome;
    }

    public record RewardActionDetectedJob(
        string UserId,
        int RewardId,
        DateTime? CollectedDate,
        DateTime? ExpiredDate,
        bool IsExpired,
        bool IsCollected) : JobInfoBase, IStatisticJobInfo
    {
        public StatisticJobType JobType => StatisticJobType.RewardActionDetected;
    }
}