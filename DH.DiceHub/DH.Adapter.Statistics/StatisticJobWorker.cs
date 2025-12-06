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
            using var scope = serviceScopeFactory.CreateScope();

            var queue = scope.ServiceProvider.GetRequiredService<IStatisticJobQueue>();
            var queuedJobService = scope.ServiceProvider.GetRequiredService<IQueuedJobService>();
            var factory = scope.ServiceProvider.GetRequiredService<IStatisticJobFactory>();

            var queuedJobs = await queue.TryDequeue(cancellationToken);
            var nextJobsForProcessing = queuedJobs
                .Select(q => DeserializeJob(q));

            foreach (var jobInfo in nextJobsForProcessing)
            {
                try
                {
                    var handler = factory.CreateHandler(jobInfo);
                    await handler.ExecuteAsync(cancellationToken);

                    await queuedJobService.UpdateStatusToCompleted(queue.QueueName, jobInfo.JobId);
                }
                catch (TaskCanceledException)
                {
                    // Application is stopping, just ignore
                    this.logger.LogInformation("StatisticJobWorker Job ID: {jobId} - Canceled at {cancelTime}.", jobInfo.JobId, DateTime.UtcNow);
                }
                catch (NotSupportedException ex)
                {
                    this.logger.LogCritical(
                        ex, "StatisticJobWorker Job ID: {jobId} - Failed at {failureTime}; Error: {error}; ReservationCleanupJobInfo: {jobInfo}",
                        jobInfo.JobId, DateTime.UtcNow, ex.Message, JsonSerializer.Serialize(jobInfo));
                }
                catch (Exception ex)
                {
                    await queuedJobService.UpdateStatusToFailed(queue.QueueName, jobInfo.JobId);

                    this.logger.LogError(ex,
                        "StatisticJobWorker Job ID: {jobId} - Failed at {failureTime}; Handler was not processed successfully; ReservationCleanupJobInfo: {jobInfo}",
                        jobInfo.JobId, DateTime.UtcNow, JsonSerializer.Serialize(jobInfo));
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
}
