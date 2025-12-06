using DH.Domain.Enums;
using DH.Domain.Queue;

namespace DH.Domain.Adapters.Statistics;

public record ClubActivityDetectedJob(string UserId, DateTime LogDate)
  : JobInfoBase(StatisticJobQueueHelper.BuildJobId(nameof(ClubActivityDetectedJob), UserId)), IStatisticJobInfo
{
    public StatisticJobType JobType => StatisticJobType.ClubActivityDetected;
}

public record ChallengeProcessingOutcomeJob(
    string UserId, int ChallengeId, ChallengeOutcome Outcome, DateTime OutcomeDate, DateTime LogDate)
    : JobInfoBase(StatisticJobQueueHelper.BuildJobId(nameof(ChallengeProcessingOutcomeJob), UserId)), IStatisticJobInfo
{
    public StatisticJobType JobType => StatisticJobType.ChallengeProcessingOutcome;
}

public record EventAttendanceDetectedJob(
    string UserId, AttendanceAction Type, int EventId, DateTime LogDate)
    : JobInfoBase(StatisticJobQueueHelper.BuildJobId(nameof(EventAttendanceDetectedJob), UserId)), IStatisticJobInfo
{
    public StatisticJobType JobType => StatisticJobType.EventAttendanceDetected;
}

public record ReservationProcessingOutcomeJob(
    string UserId,
    ReservationOutcome Outcome,
    ReservationType Type,
    int ReservationId,
    DateTime OutcomeDate) : JobInfoBase(StatisticJobQueueHelper.BuildJobId(nameof(ReservationProcessingOutcomeJob), UserId)), IStatisticJobInfo
{
    public StatisticJobType JobType => StatisticJobType.ReservationProcessingOutcome;
}

public record RewardActionDetectedJob(
    string UserId,
    int RewardId,
    DateTime? CollectedDate,
    DateTime? ExpiredDate,
    bool IsExpired,
    bool IsCollected) : JobInfoBase(StatisticJobQueueHelper.BuildJobId(nameof(RewardActionDetectedJob), UserId)), IStatisticJobInfo
{
    public StatisticJobType JobType => StatisticJobType.RewardActionDetected;
}

public record GameEngagementDetectedJob(
    string UserId,
    int GameId,
    DateTime DetectedOn) : JobInfoBase(StatisticJobQueueHelper.BuildJobId(nameof(GameEngagementDetectedJob), UserId)), IStatisticJobInfo
{
    public StatisticJobType JobType => StatisticJobType.GameEngagementDetected;
}
