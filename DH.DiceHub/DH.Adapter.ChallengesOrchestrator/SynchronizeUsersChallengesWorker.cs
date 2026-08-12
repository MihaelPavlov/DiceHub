using DH.Adapter.Authentication.Helper;
using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.ChallengesOrchestrator;
using DH.Domain.Entities;
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
            using var scope = this.serviceScopeFactory.CreateScope();
            var queue = scope.ServiceProvider.GetRequiredService<ISynchronizeUsersChallengesQueue>();

            var nextJobsForProcessing = (await queue.TryDequeue(cancellationToken))
                .Select(q => new { Job = q, Payload = JsonSerializer.Deserialize<JobInfo>(q.MessagePayload)! })
                .ToList();

            var queuedJobService = scope.ServiceProvider.GetRequiredService<IQueuedJobService>();
            var userChallengesManagementService = scope.ServiceProvider.GetRequiredService<IUserChallengesManagementService>();
            foreach (var nextJob in nextJobsForProcessing)
            {
                SetTenantExecutionContext(scope, nextJob.Job.TenantId);
                string traceId = Guid.NewGuid().ToString();

                try
                {
                    var jobStartTime = DateTime.UtcNow;
                    this.logger.LogInformation("Trace Id: {traceId}; Job Id: {jobId} - Started at {startTime} - Job Info: {jobInfo}", traceId, nextJob.Payload.JobId, jobStartTime, JsonSerializer.Serialize(nextJob.Payload));

                    switch (nextJob.Payload.TypeOfJob)
                    {
                        case nameof(SynchronizeNewUserJob):
                            await userChallengesManagementService.InitiateUserChallengePeriod(nextJob.Payload.UserId, cancellationToken, forNewUser: true);
                            await queuedJobService.UpdateStatusToCompleted(queue.QueueName, nextJob.Payload.JobId);
                            break;
                        case nameof(ChallengeInitiationJob):
                            if (nextJob.Payload.ScheduledTime.HasValue && DateTime.UtcNow >= nextJob.Payload.ScheduledTime)
                            {
                                await userChallengesManagementService.AssignNextChallengeToUserAsync(nextJob.Payload.UserId, cancellationToken);
                                await queuedJobService.UpdateStatusToCompleted(queue.QueueName, nextJob.Payload.JobId);
                                break;
                            }

                            this.logger.LogInformation("Trace Id: {traceId}; Job Id: {jobId} - Requeued at {requeueTime} - Job Info: {jobInfo}",
                                traceId, nextJob.Payload.JobId, DateTime.UtcNow, JsonSerializer.Serialize(nextJob.Payload));
                            break;
                        default:
                            this.logger.LogWarning("Trace Id: {traceId}; Job Id: {jobId} - Unknown job type at {warningTime}: {jobInfo}",
                                traceId, nextJob.Payload.JobId, DateTime.UtcNow, JsonSerializer.Serialize(nextJob.Payload));
                            break;
                    }

                    DateTime jobEndTime = DateTime.UtcNow;
                    this.logger.LogInformation("Trace Id: {traceId}; Job Id: {jobId} - Ended at {endTime} - Duration: {duration} - Job Info: {jobInfo}",
                        traceId, nextJob.Payload.JobId, jobEndTime, (jobEndTime - jobStartTime).TotalMilliseconds, JsonSerializer.Serialize(nextJob.Payload));
                }
                catch (TaskCanceledException)
                {
                    this.logger.LogInformation("Trace Id: {traceId}; Job Id: {jobId} - Canceled at {cancelTime}.", traceId, nextJob.Payload.JobId, DateTime.UtcNow);
                }
                catch (Exception ex)
                {
                    await queuedJobService.UpdateStatusToFailed(queue.QueueName, nextJob.Payload.JobId);
                    this.logger.LogError(ex, "Trace Id: {traceId}; Job Id: {jobId} - Failed at {failureTime}: {jobInfo}", traceId, nextJob.Payload.JobId, DateTime.UtcNow, JsonSerializer.Serialize(nextJob.Payload));
                }
                finally
                {
                    ClearTenantExecutionContext(scope);
                }
            }

            if (!nextJobsForProcessing.Any())
                await Task.Delay(TimeSpan.FromMinutes(5), cancellationToken);
        }
    }

    private static void SetTenantExecutionContext(IServiceScope scope, string tenantId)
    {
        scope.ServiceProvider.GetRequiredService<ITenantExecutionContextAccessor>().TenantId = tenantId;
        scope.ServiceProvider.GetRequiredService<ISystemUserContextAccessor>().Set(new SystemUserContext(tenantId, "system-worker"));
    }

    private static void ClearTenantExecutionContext(IServiceScope scope)
    {
        scope.ServiceProvider.GetRequiredService<ITenantExecutionContextAccessor>().Clear();
    }
}
