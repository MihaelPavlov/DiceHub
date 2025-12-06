using DH.Domain.Queue;
using DH.Domain.Services.Queue;
using System.Text.Json;

namespace DH.Domain.Adapters.Statistics.Services;

public class StatisticQueuePublisher(IQueuedJobService queuedJobService) : IStatisticQueuePublisher
{
    readonly IQueuedJobService queuedJobService = queuedJobService;

    public Task PublishAsync<TJob>(TJob job)
        where TJob : IStatisticJobInfo
    {
        return queuedJobService.Create(
            QueueNameKeysConstants.STATISTICS_QUEUE_NAME,
            job.JobId,
            JsonSerializer.Serialize(job),
            job.JobType.ToString());
    }
}
