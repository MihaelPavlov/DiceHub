using DH.Domain.Adapters.Statistics;
using DH.Domain.Services.Queue;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace DH.Adapter.Statistics;

public class StatisticJobWorker : BackgroundService
{
    readonly ILogger<StatisticJobWorker> logger;
    readonly StatisticJobQueue queue;
    readonly IServiceScopeFactory serviceScopeFactory;

    public StatisticJobWorker(ILogger<StatisticJobWorker> logger, IServiceScopeFactory serviceScopeFactory, StatisticJobQueue queue)
    {
        this.logger = logger;
        this.serviceScopeFactory = serviceScopeFactory;
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
                var factory = scope.ServiceProvider.GetRequiredService<IStatisticJobFactory>();

                try
                {
                    Console.WriteLine($"Executing job {jobInfo.JobId}...");
                    var hadnler = factory.CreateHandler(jobInfo);
                    var isSuccessfully = await hadnler.ExecuteAsync(cancellationToken);

                    if (isSuccessfully)
                    {
                        await queuedJobService.UpdateStatusToCompleted(this.queue.QueueName, jobInfo.JobId);
                    }
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
