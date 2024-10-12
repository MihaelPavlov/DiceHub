using DH.Domain.Adapters.ChallengesOrchestrator;
using DH.Domain.Adapters.Data;
using DH.Domain.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace DH.Adapter.ChallengesOrchestrator;

internal class SynchronizeUsersChallengesService : BackgroundService
{
    readonly IUserChallengesManagementService userChallengesManagementService;
    readonly ILogger<SynchronizeUsersChallengesService> logger;
    readonly SynchronizeUsersChallengesQueue queue;

    public SynchronizeUsersChallengesService(ITenantDbContext tenantDbContext, ILogger<SynchronizeUsersChallengesService> logger, SynchronizeUsersChallengesQueue queue, IUserChallengesManagementService userChallengesManagementService)
    {
        this.userChallengesManagementService = userChallengesManagementService;
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
                    var jobStartTime = DateTime.UtcNow;
                    logger.LogInformation("Job ID: {jobId} - Started at {startTime} - Job Info: {jobInfo}", traceId, jobStartTime, JsonSerializer.Serialize(jobInfo));

                    switch (jobInfo)
                    {
                        case SynchronizeUsersChallengesQueue.SynchronizeNewUserJob newUserJob:
                            await this.userChallengesManagementService.InitiateNewUserChallenges(newUserJob.UserId, cancellationToken);
                            break;
                        case SynchronizeUsersChallengesQueue.ChallengeInitiationJob challengeJob:
                            if (challengeJob.ScheduledTime.HasValue && DateTime.UtcNow >= challengeJob.ScheduledTime)
                            {
                                await this.userChallengesManagementService.AddChallengeToUser(challengeJob.UserId, cancellationToken);
                                break;
                            }
                            this.queue.AddChallengeInitiationJob(challengeJob.UserId, challengeJob.ScheduledTime.GetValueOrDefault());
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
}
