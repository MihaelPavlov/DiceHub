using DH.Domain.Adapters.ChallengesOrchestrator;
using DH.Domain.Services;
using DH.Domain.Services.Queue;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using static System.Formats.Asn1.AsnWriter;

namespace DH.Adapter.ChallengesOrchestrator;

public class SynchronizeUsersChallengesService : BackgroundService
{
    readonly ILogger<SynchronizeUsersChallengesService> logger;
    readonly SynchronizeUsersChallengesQueue queue;
    readonly IServiceScopeFactory serviceScopeFactory;

    public SynchronizeUsersChallengesService(IServiceScopeFactory serviceScopeFactory, ILogger<SynchronizeUsersChallengesService> logger, SynchronizeUsersChallengesQueue queue)
    {
        this.serviceScopeFactory = serviceScopeFactory;
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
                using var scope = serviceScopeFactory.CreateScope();
                var queuedJobService = scope.ServiceProvider.GetRequiredService<IQueuedJobService>();
                try
                {
                    var jobStartTime = DateTime.UtcNow;
                    logger.LogInformation("Job ID: {jobId} - Started at {startTime} - Job Info: {jobInfo}", traceId, jobStartTime, JsonSerializer.Serialize(jobInfo));

                    var userChallengesManagementService = scope.ServiceProvider.GetRequiredService<IUserChallengesManagementService>();
                    switch (jobInfo)
                    {
                        case SynchronizeUsersChallengesQueue.SynchronizeNewUserJob newUserJob:
                            await userChallengesManagementService.InitiateNewUserChallengePeriod(newUserJob.UserId, cancellationToken);

                            await queuedJobService.UpdateStatusToCompleted(this.queue.QueueName, jobInfo.JobId);
                            break;
                        case SynchronizeUsersChallengesQueue.ChallengeInitiationJob challengeJob:
                            if (challengeJob.ScheduledTime.HasValue && DateTime.UtcNow >= challengeJob.ScheduledTime)
                            {
                                await userChallengesManagementService.AddChallengeToUser(challengeJob.UserId, cancellationToken);

                                await queuedJobService.UpdateStatusToCompleted(this.queue.QueueName, jobInfo.JobId);
                                break;
                            }
                            this.queue.RequeueChallengeInitiationJob(challengeJob);
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
                    await queuedJobService.UpdateStatusToFailed(this.queue.QueueName, jobInfo.JobId);

                    logger.LogError(ex, "Job ID: {jobId} - Failed at {failureTime}: {jobInfo}", traceId, DateTime.UtcNow, JsonSerializer.Serialize(jobInfo));
                }
            }

            await Task.Delay(10000, cancellationToken);
        }
    }
}
