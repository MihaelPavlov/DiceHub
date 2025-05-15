using DH.Domain.Adapters.ChallengesOrchestrator;
using DH.Domain.Adapters.GameSession;
using DH.Domain.Adapters.Reservations;
using DH.Domain.Adapters.Statistics;
using DH.Domain.Services.Queue;
using System.Text.Json;
using static DH.Domain.Adapters.Statistics.StatisticJobQueue;

namespace DH.Domain.Queue.Services;

public class QueueDispatcher : IQueueDispatcher
{
    readonly SynchronizeGameSessionQueue gameSessionQueue;
    readonly SynchronizeUsersChallengesQueue usersChallengesQueue;
    readonly ReservationCleanupQueue reservationCleanupQueue;
    readonly StatisticJobQueue statisticsQueue;
    readonly IQueuedJobService queuedJobService;
    readonly IStatisticJobFactory statiscticFactory;
    public QueueDispatcher(
        SynchronizeGameSessionQueue gameSessionQueue,
        SynchronizeUsersChallengesQueue usersChallengesQueue,
        ReservationCleanupQueue reservationCleanupQueue,
        StatisticJobQueue statisticsQueue,
        IQueuedJobService queuedJobService,
        IStatisticJobFactory statiscticFactory)
    {
        this.gameSessionQueue = gameSessionQueue;
        this.usersChallengesQueue = usersChallengesQueue;
        this.reservationCleanupQueue = reservationCleanupQueue;
        this.queuedJobService = queuedJobService;
        this.statiscticFactory = statiscticFactory;
        this.statisticsQueue = statisticsQueue;
    }

    public void Dispatch()
    {
        var queuedJobs = this.queuedJobService.GetJobsInPendingStatus();
        foreach (var queuedJob in queuedJobs)
        {
            switch (queuedJob.QueueType)
            {
                case QueueNameKeysConstants.SYNCHRONIZE_GAME_SESSION_QUEUE_NAME:
                    var gameSessionJob = JsonSerializer.Deserialize<SynchronizeGameSessionQueue.JobInfo>(queuedJob.MessagePayload);
                    if (gameSessionJob != null)
                    {
                        gameSessionQueue.RequeueJob(gameSessionJob);
                    }
                    break;
                case QueueNameKeysConstants.SYNCHRONIZE_USERS_CHALLENGES_QUEUE_NAME:
                    DispatchUsersChallengesJob(usersChallengesQueue, queuedJob.MessagePayload);
                    break;
                case QueueNameKeysConstants.STATISTICS_QUEUE_NAME:
                    DispatchStatisticsJob(statisticsQueue, queuedJob.JobType, queuedJob.MessagePayload);
                    break;
                case QueueNameKeysConstants.RESERVATION_CLEANUP_QUEUE_NAME:
                    var reservationCleanupJob = JsonSerializer.Deserialize<ReservationCleanupQueue.JobInfo>(queuedJob.MessagePayload);
                    if (reservationCleanupJob != null)
                    {
                        reservationCleanupQueue.RequeueJob(reservationCleanupJob);
                    }
                    break;
                default:
                    throw new ArgumentException($"Unknown queue type: {queuedJob.QueueType}");
            }
        }
    }

    private void DispatchStatisticsJob(StatisticJobQueue queue, string jobType, string messagePayload)
    {
        if (!Enum.TryParse<StatisticJobType>(jobType, out var parsedType))
            throw new InvalidOperationException($"Unknown job type: {jobType}");

        IStatisticJobInfo? jobInfo = parsedType switch
        {
            StatisticJobType.ClubActivityDetected =>
                JsonSerializer.Deserialize<ClubActivityDetectedJob>(messagePayload),

            StatisticJobType.ChallengeProcessingOutcome =>
                JsonSerializer.Deserialize<ChallengeOutcomeJob>(messagePayload),

            _ => throw new NotSupportedException($"Job type not supported: {parsedType}")
        };

        if (jobInfo is null)
            throw new InvalidOperationException("Deserialization failed");

        var handler = this.statiscticFactory.CreateHandler(jobInfo);
        queue.Enqueue(handler);

    }
    private void DispatchUsersChallengesJob(SynchronizeUsersChallengesQueue queue, string messagePayload)
    {
        // Try to deserialize into the base JobInfo type first
        var baseJobInfo = JsonSerializer.Deserialize<SynchronizeUsersChallengesQueue.JobInfo>(messagePayload);
        if (baseJobInfo == null)
        {
            throw new InvalidOperationException("Failed to deserialize job payload.");
        }

        // Determine the specific job type based on the payload
        if (baseJobInfo.ScheduledTime != null)
        {
            // Deserialize as ChallengeInitiationJob
            var challengeInitiationJob = JsonSerializer.Deserialize<SynchronizeUsersChallengesQueue.ChallengeInitiationJob>(messagePayload);
            if (challengeInitiationJob != null)
            {
                queue.RequeueChallengeInitiationJob(challengeInitiationJob);
            }
        }
        else
        {
            // Deserialize as SynchronizeNewUserJob
            var synchronizeNewUserJob = JsonSerializer.Deserialize<SynchronizeUsersChallengesQueue.SynchronizeNewUserJob>(messagePayload);
            if (synchronizeNewUserJob != null)
            {
                queue.RequeueSynchronizeNewUserJob(synchronizeNewUserJob);
            }
        }
    }
}
