using DH.Domain.Services.Queue;
using System.Text.Json;

namespace DH.Domain.Adapters.Statistics.Services;

public class StatisticQueuePublisher(StatisticJobQueue queue, IQueuedJobService queuedJobService) : IStatisticQueuePublisher
{
    readonly StatisticJobQueue queue = queue;
    readonly IQueuedJobService queuedJobService = queuedJobService;

    public Task PublishAsync<TJob>(TJob job)
        where TJob : IStatisticJobInfo
    {
        queue.Enqueue(job);
        return queuedJobService.Create(
            queue.QueueName,
            job.JobId,
            JsonSerializer.Serialize(job),
            job.JobType.ToString());
    }
}
