using DH.Domain.Adapters.ChallengesOrchestrator;
using DH.Domain.Adapters.Authentication;
using DH.Domain.Services;
using DH.Domain.Services.Queue;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace DH.Adapter.ChallengesOrchestrator;

public class SynchronizeUsersChallengesWorker : BackgroundService
{
    readonly ILogger<SynchronizeUsersChallengesWorker> logger;
    readonly IServiceScopeFactory serviceScopeFactory;

    public SynchronizeUsersChallengesWorker(IServiceScopeFactory serviceScopeFactory, ILogger<SynchronizeUsersChallengesWorker> logger)
    {
        this.serviceScopeFactory = serviceScopeFactory;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var queue = scope.ServiceProvider.GetRequiredService<ISynchronizeUsersChallengesQueue>();

            var queuedJobs = await queue.TryDequeue(cancellationToken);

            var nextJobsForProcessing = queuedJobs
                .Select(q => JsonSerializer.Deserialize<JobInfo>(q.MessagePayload)!);
            var tenantIdsByJobId = queuedJobs.ToDictionary(q => q.JobId, q => q.TenantId);

            var queuedJobService = scope.ServiceProvider.GetRequiredService<IQueuedJobService>();
            var userChallengesManagementService = scope.ServiceProvider.GetRequiredService<IUserChallengesManagementService>();
            var systemUserContextAccessor = scope.ServiceProvider.GetRequiredService<ISystemUserContextAccessor>();
            foreach (var nextJob in nextJobsForProcessing)
            {
                string traceId = Guid.NewGuid().ToString();

                try
                {
                    if (!tenantIdsByJobId.TryGetValue(nextJob.JobId, out var tenantId) || string.IsNullOrWhiteSpace(tenantId))
                    {
                        throw new InvalidOperationException($"Queued job {nextJob.JobId} has no tenant id.");
                    }

                    systemUserContextAccessor.Set(new WorkerSystemUserContext(tenantId));

                    var jobStartTime = DateTime.UtcNow;
                    logger.LogInformation("Trace Id: {traceId}; Job Id: {jobId} - Started at {startTime} - Job Info: {jobInfo}", traceId, nextJob.JobId, jobStartTime, JsonSerializer.Serialize(nextJob));

                    switch (nextJob.TypeOfJob)
                    {
                        case nameof(SynchronizeNewUserJob):
                            var periodCreated = await userChallengesManagementService
                                .InitiateUserChallengePeriod(nextJob.UserId, cancellationToken, forNewUser: true);

                            if (periodCreated)
                                await queuedJobService.UpdateStatusToCompleted(queue.QueueName, nextJob.JobId);
                            else
                                await queuedJobService.UpdateStatusToFailed(queue.QueueName, nextJob.JobId);
                            break;
                        case nameof(ChallengeInitiationJob):
                            if (nextJob.ScheduledTime.HasValue && DateTime.UtcNow >= nextJob.ScheduledTime)
                            {
                                await userChallengesManagementService.AssignNextChallengeToUserAsync(nextJob.UserId, cancellationToken);

                                await queuedJobService.UpdateStatusToCompleted(queue.QueueName, nextJob.JobId);
                                break;
                            }

                            logger.LogInformation("Trace Id: {traceId}; Job Id: {jobId} - Requeued at {requeueTime} - Job Info: {jobInfo}",
                                traceId, nextJob.JobId, DateTime.UtcNow, JsonSerializer.Serialize(nextJob));
                            break;
                        default:
                            logger.LogWarning("Trace Id: {traceId}; Job Id: {jobId} - Unknown job type at {warningTime}: {jobInfo}",
                                traceId, nextJob.JobId, DateTime.UtcNow, JsonSerializer.Serialize(nextJob));
                            break;
                    }

                    DateTime jobEndTime = DateTime.UtcNow;
                    logger.LogInformation("Trace Id: {traceId}; Job Id: {jobId} - Ended at {endTime} - Duration: {duration} - Job Info: {jobInfo}",
                        traceId, nextJob.JobId, jobEndTime, (jobEndTime - jobStartTime).TotalMilliseconds, JsonSerializer.Serialize(nextJob));
                }
                catch (TaskCanceledException)
                {
                    // Application is stopping, just ignore
                    logger.LogInformation("Trace Id: {traceId}; Job Id: {jobId} - Canceled at {cancelTime}.", traceId, nextJob.JobId, DateTime.UtcNow);
                }
                catch (Exception ex)
                {
                    await queuedJobService.UpdateStatusToFailed(queue.QueueName, nextJob.JobId);

                    logger.LogError(ex, "Trace Id: {traceId}; Job Id: {jobId} - Failed at {failureTime}: {jobInfo}", traceId, nextJob.JobId, DateTime.UtcNow, JsonSerializer.Serialize(nextJob));
                }
            }

            if (!nextJobsForProcessing.Any())
                await Task.Delay(TimeSpan.FromMinutes(5), cancellationToken);
        }
    }

    private sealed class WorkerSystemUserContext(string tenantId) : IUserContext
    {
        public string? TenantId => tenantId;
        public string? UserId => "challenges-orchestrator";
        public int? RoleKey => null;
        public string? TimeZone => "UTC";
        public string? Language => "en";
        public bool IsAuthenticated => false;
        public bool IsSystem => true;
    }
}
