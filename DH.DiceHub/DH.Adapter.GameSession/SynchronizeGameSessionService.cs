using DH.Domain.Adapters.GameSession;
using DH.Domain.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using static DH.Domain.Adapters.GameSession.SynchronizeGameSessionQueue;

namespace DH.Adapter.GameSession;

public class SynchronizeGameSessionService : BackgroundService
{
    readonly IGameSessionService gameSessionService;
    readonly ILogger<SynchronizeGameSessionService> logger;
    readonly SynchronizeGameSessionQueue queue;

    public SynchronizeGameSessionService(IGameSessionService gameSessionService, ILogger<SynchronizeGameSessionService> logger, SynchronizeGameSessionQueue queue)
    {
        this.gameSessionService = gameSessionService;
        this.logger = logger;
        this.queue = queue;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (this.queue.TryDequeue(out var jobInfo))
            {
                string traceId = Guid.NewGuid().ToString();

                try
                {
                    if (IsJobCanceled(jobInfo))
                    {
                        logger.LogInformation("Job ID: {jobId} - Skipped processing because it is canceled by the user - Job Info: {jobInfo}", traceId, JsonSerializer.Serialize(jobInfo));
                        continue;
                    }

                    var jobStartTime = DateTime.UtcNow;
                    logger.LogInformation("Job ID: {jobId} - Started at {startTime} - Job Info: {jobInfo}", traceId, jobStartTime, JsonSerializer.Serialize(jobInfo));

                    switch (jobInfo)
                    {
                        case SynchronizeGameSessionQueue.UserPlayTimeEnforcerJob enforcerJob:
                            if (DateTime.UtcNow >= enforcerJob.RequiredPlayUntil)
                            {
                                await ProcessUserPlayTimeEnforcerJob(enforcerJob, traceId, cancellationToken);
                                break;
                            }
                            this.queue.AddUserPlayTimEnforcerJob(enforcerJob.UserId, enforcerJob.GameId, enforcerJob.RequiredPlayUntil);
                            logger.LogInformation("Job ID: {jobId} - Requeued at {requeueTime} - Job Info: {jobInfo}", traceId, DateTime.UtcNow, JsonSerializer.Serialize(jobInfo));
                            break;
                        default:
                            logger.LogWarning("Job ID: {jobId} - Unknown job type at {warningTime}: {jobInfo}", traceId, DateTime.UtcNow, JsonSerializer.Serialize(jobInfo));
                            break;
                    }

                    DateTime jobEndTime = DateTime.UtcNow;
                    logger.LogInformation("Job ID: {jobId} - Ended at {endTime} - Duration: {duration} - Job Info: {jobInfo}", traceId, jobEndTime, (jobEndTime - jobStartTime).TotalMilliseconds, JsonSerializer.Serialize(jobInfo));
                }
                catch (TaskCanceledException)
                {
                    // Application is stopping, just ignore
                    logger.LogInformation("Job ID: {jobId} - Canceled at {cancelTime}.", traceId, DateTime.UtcNow);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Job ID: {jobId} - Failed at {failureTime}: {jobInfo}", traceId, DateTime.UtcNow, JsonSerializer.Serialize(jobInfo));
                }
            }

            await Task.Delay(1000, cancellationToken);
        }
    }

    private async Task ProcessUserPlayTimeEnforcerJob(UserPlayTimeEnforcerJob enforcerJob, string traceId, CancellationToken cancellationToken)
    {
        bool isProcessSuccessful = false;
        bool isEvaluationSuccessful = false;

        try
        {
            isProcessSuccessful = await this.gameSessionService.ProcessChallengeAfterSession(enforcerJob.UserId, enforcerJob.GameId, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Job ID: {jobId} - Failed during ProcessChallengeAfterSession at {failureTime}: {jobInfo}", traceId, DateTime.UtcNow, JsonSerializer.Serialize(enforcerJob));
            throw;
        }

        if (isProcessSuccessful)
        {
            try
            {
                isEvaluationSuccessful = await this.gameSessionService.EvaluateRewardsAfterChallenges(enforcerJob.UserId, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Job ID: {jobId} - Failed during EvaluateRewardsAfterChallenges at {failureTime}: {jobInfo}", traceId, DateTime.UtcNow, JsonSerializer.Serialize(enforcerJob));
                throw;
            }

            if (!isEvaluationSuccessful)
            {
                logger.LogWarning("Job ID: {jobId} - Nothing for reward evaluation at {currentTime} - Job Info: {jobInfo}", traceId, DateTime.UtcNow, JsonSerializer.Serialize(enforcerJob));
            }
        }
        else
        {
            logger.LogWarning("Job ID: {jobId} - this.gameSessionService.ProcessChallengeAfterSession the user doesn't have anything to process at {currentTime} - Job Info: {jobInfo}", traceId, DateTime.UtcNow, JsonSerializer.Serialize(enforcerJob));
        }
    }

    /// <summary>
    /// The IsJobCanceled method checks whether a specific job has been canceled.
    /// </summary>
    /// <param name="job">Parameter which contains information about the job to be checked.</param>
    /// <returns>True if the job is canceled; otherwise, false.</returns>
    private bool IsJobCanceled(JobInfo job)
    {
        return this.queue.CanceledJobs.Contains((job.UserId, job.GameId));
    }
}
