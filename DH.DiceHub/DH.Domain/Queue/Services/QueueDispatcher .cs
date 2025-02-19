using DH.Domain.Adapters.ChallengesOrchestrator;
using DH.Domain.Adapters.GameSession;
using DH.Domain.Adapters.Reservations;
using DH.Domain.Services.Queue;
using System.Text.Json;

namespace DH.Domain.Queue.Services;

public class QueueDispatcher : IQueueDispatcher
{
    readonly SynchronizeGameSessionQueue gameSessionQueue;
    readonly SynchronizeUsersChallengesQueue usersChallengesQueue;
    readonly ReservationCleanupQueue reservationCleanupQueue;
    readonly IQueuedJobService queuedJobService;

    public QueueDispatcher(SynchronizeGameSessionQueue gameSessionQueue, SynchronizeUsersChallengesQueue usersChallengesQueue, ReservationCleanupQueue reservationCleanupQueue, IQueuedJobService queuedJobService)
    {
        this.gameSessionQueue = gameSessionQueue;
        this.usersChallengesQueue = usersChallengesQueue;
        this.reservationCleanupQueue = reservationCleanupQueue;
        this.queuedJobService = queuedJobService;
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
