using DH.Adapter.Authentication.Helper;
using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.GameSession;
using DH.Domain.Entities;
using DH.Domain.Services;
using DH.Domain.Services.Queue;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace DH.Adapter.GameSession;

public class GameSessionWorker : BackgroundService
{
    readonly ILogger<GameSessionWorker> logger;
    readonly IServiceScopeFactory serviceScopeFactory;

    public GameSessionWorker(IServiceScopeFactory serviceScopeFactory, ILogger<GameSessionWorker> logger)
    {
        this.serviceScopeFactory = serviceScopeFactory;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            using var outerScope = this.serviceScopeFactory.CreateScope();
            var queue = outerScope.ServiceProvider.GetRequiredService<IGameSessionQueue>();

            var queuedJobs = await queue.TryDequeue(cancellationToken);
            var nextJobsForProcessing = queuedJobs
                .Select(q => new { Job = q, Payload = JsonSerializer.Deserialize<UserPlayTimeEnforcerJob>(q.MessagePayload)! })
                .Where(x => DateTime.UtcNow > x.Payload.RequiredPlayUntil)
                .ToList();

            var tasks = new List<Task>();
            foreach (var nextJob in nextJobsForProcessing)
            {
                tasks.Add(Task.Run(async () =>
                {
                    using var scope = this.serviceScopeFactory.CreateScope();
                    await this.ProcessJobAsync(nextJob.Job, nextJob.Payload, queue.QueueName, scope, cancellationToken);
                }, cancellationToken));
            }

            if (tasks.Count > 0)
                await Task.WhenAll(tasks);

            if (!nextJobsForProcessing.Any())
                await Task.Delay(TimeSpan.FromMinutes(3), cancellationToken);
        }
    }

    private async Task ProcessJobAsync(QueuedJob queuedJob, UserPlayTimeEnforcerJob jobInfo, string queueName, IServiceScope scope, CancellationToken cancellationToken)
    {
        string traceId = Guid.NewGuid().ToString();
        var queuedJobService = scope.ServiceProvider.GetRequiredService<IQueuedJobService>();

        SetTenantExecutionContext(scope, queuedJob.TenantId);

        try
        {
            var jobStartTime = DateTime.UtcNow;
            this.logger.LogInformation("Job ID: {jobId} - Started at {startTime} - Job Info: {jobInfo}", traceId, jobStartTime, JsonSerializer.Serialize(jobInfo));

            await this.ProcessUserPlayTimeEnforcerJob(scope, jobInfo, traceId, cancellationToken);
            await queuedJobService.UpdateStatusToCompleted(queueName, jobInfo.JobId);

            DateTime jobEndTime = DateTime.UtcNow;
            this.logger.LogInformation("Job ID: {jobId} - Ended at {endTime} - Duration: {duration} - Job Info: {jobInfo}", traceId, jobEndTime, (jobEndTime - jobStartTime).TotalMilliseconds, JsonSerializer.Serialize(jobInfo));
        }
        catch (TaskCanceledException)
        {
            this.logger.LogInformation("Job ID: {jobId} - Canceled at {cancelTime}.", traceId, DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            await queuedJobService.UpdateStatusToFailed(queueName, jobInfo.JobId);
            this.logger.LogError(ex, "Job ID: {jobId} - Failed at {failureTime}: {jobInfo}", traceId, DateTime.UtcNow, JsonSerializer.Serialize(jobInfo));
        }
        finally
        {
            ClearTenantExecutionContext(scope);
        }
    }

    private async Task ProcessUserPlayTimeEnforcerJob(IServiceScope scope, UserPlayTimeEnforcerJob enforcerJob, string traceId, CancellationToken cancellationToken)
    {
        bool isProcessSuccessful = false;
        bool isCollectingSuccessful = false;

        var gameSessionService = scope.ServiceProvider.GetRequiredService<IGameSessionService>();

        try
        {
            isProcessSuccessful = await gameSessionService.ProcessChallengeAfterSession(enforcerJob.UserId, enforcerJob.GameId, cancellationToken);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Job ID: {jobId} - Failed during ProcessChallengeAfterSession at {failureTime}: {jobInfo}", traceId, DateTime.UtcNow, JsonSerializer.Serialize(enforcerJob));
            throw;
        }

        if (isProcessSuccessful)
        {
            try
            {
                isCollectingSuccessful = await gameSessionService.CollectRewardsFromChallenges(enforcerJob.UserId, cancellationToken);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Job ID: {jobId} - Failed during CollectRewardsFromChallenges at {failureTime}: {jobInfo}", traceId, DateTime.UtcNow, JsonSerializer.Serialize(enforcerJob));
                throw;
            }

            if (!isCollectingSuccessful)
            {
                this.logger.LogWarning("Job ID: {jobId} - Nothing for collecting from challenges at {currentTime} - Job Info: {jobInfo}", traceId, DateTime.UtcNow, JsonSerializer.Serialize(enforcerJob));
            }
            else
            {
                try
                {
                    await gameSessionService.EvaluateUserRewards(enforcerJob.UserId, cancellationToken);
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, "Job ID: {jobId} - Failed during EvaluateUserRewards at {failureTime}: {jobInfo}", traceId, DateTime.UtcNow, JsonSerializer.Serialize(enforcerJob));
                    throw;
                }
            }
        }
        else
        {
            this.logger.LogWarning("Job ID: {jobId} - this.gameSessionService.ProcessChallengeAfterSession the user doesn't have anything to process at {currentTime} - Job Info: {jobInfo}", traceId, DateTime.UtcNow, JsonSerializer.Serialize(enforcerJob));
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
