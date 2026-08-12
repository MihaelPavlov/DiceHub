using DH.Adapter.Authentication.Helper;
using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Statistics;
using DH.Domain.Entities;
using DH.Domain.Services.Queue;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace DH.Adapter.Statistics;

public class StatisticJobWorker : BackgroundService
{
    readonly ILogger<StatisticJobWorker> logger;
    readonly IServiceScopeFactory serviceScopeFactory;

    public StatisticJobWorker(ILogger<StatisticJobWorker> logger, IServiceScopeFactory serviceScopeFactory)
    {
        this.logger = logger;
        this.serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            using var scope = this.serviceScopeFactory.CreateScope();

            var queue = scope.ServiceProvider.GetRequiredService<IStatisticJobQueue>();
            var queuedJobService = scope.ServiceProvider.GetRequiredService<IQueuedJobService>();
            var factory = scope.ServiceProvider.GetRequiredService<IStatisticJobFactory>();

            var nextJobsForProcessing = (await queue.TryDequeue(cancellationToken))
                .Select(q => new { Job = q, Payload = DeserializeJob(q) })
                .ToList();

            foreach (var jobInfo in nextJobsForProcessing)
            {
                SetTenantExecutionContext(scope, jobInfo.Job.TenantId);
                try
                {
                    var handler = factory.CreateHandler(jobInfo.Payload);
                    await handler.ExecuteAsync(cancellationToken);

                    await queuedJobService.UpdateStatusToCompleted(queue.QueueName, jobInfo.Payload.JobId);
                }
                catch (TaskCanceledException)
                {
                    this.logger.LogInformation("StatisticJobWorker Job ID: {jobId} - Canceled at {cancelTime}.", jobInfo.Payload.JobId, DateTime.UtcNow);
                }
                catch (NotSupportedException ex)
                {
                    this.logger.LogCritical(
                        ex, "StatisticJobWorker Job ID: {jobId} - Failed at {failureTime}; Error: {error}; ReservationCleanupJobInfo: {jobInfo}",
                        jobInfo.Payload.JobId, DateTime.UtcNow, ex.Message, JsonSerializer.Serialize(jobInfo.Payload));
                }
                catch (Exception ex)
                {
                    await queuedJobService.UpdateStatusToFailed(queue.QueueName, jobInfo.Payload.JobId);

                    this.logger.LogError(ex,
                        "StatisticJobWorker Job ID: {jobId} - Failed at {failureTime}; Handler was not processed successfully; ReservationCleanupJobInfo: {jobInfo}",
                        jobInfo.Payload.JobId, DateTime.UtcNow, JsonSerializer.Serialize(jobInfo.Payload));
                }
                finally
                {
                    ClearTenantExecutionContext(scope);
                }
            }

            if (!nextJobsForProcessing.Any())
                await Task.Delay(TimeSpan.FromMinutes(6), cancellationToken);
        }
    }

    private IStatisticJobInfo DeserializeJob(QueuedJob job)
    {
        var id = job.JobId;

        if (id.Contains(nameof(ClubActivityDetectedJob)))
            return JsonSerializer.Deserialize<ClubActivityDetectedJob>(job.MessagePayload)!;

        if (id.Contains(nameof(ChallengeProcessingOutcomeJob)))
            return JsonSerializer.Deserialize<ChallengeProcessingOutcomeJob>(job.MessagePayload)!;

        if (id.Contains(nameof(EventAttendanceDetectedJob)))
            return JsonSerializer.Deserialize<EventAttendanceDetectedJob>(job.MessagePayload)!;

        if (id.Contains(nameof(ReservationProcessingOutcomeJob)))
            return JsonSerializer.Deserialize<ReservationProcessingOutcomeJob>(job.MessagePayload)!;

        if (id.Contains(nameof(RewardActionDetectedJob)))
            return JsonSerializer.Deserialize<RewardActionDetectedJob>(job.MessagePayload)!;

        if (id.Contains(nameof(GameEngagementDetectedJob)))
            return JsonSerializer.Deserialize<GameEngagementDetectedJob>(job.MessagePayload)!;

        this.logger.LogWarning($"StatisticJobWorker cannot determine job type from JobId '{job.JobId}'.");

        return null!;
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
