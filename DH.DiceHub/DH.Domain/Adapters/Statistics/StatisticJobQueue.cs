using DH.Domain.Entities;
using DH.Domain.Queue;
using DH.Domain.Services.Queue;

namespace DH.Domain.Adapters.Statistics;

public class StatisticJobQueue(IQueuedJobService queuedJobService) : IStatisticJobQueue
{
    public string QueueName => QueueNameKeysConstants.STATISTICS_QUEUE_NAME;
    readonly IQueuedJobService queuedJobService = queuedJobService;

    public async Task<List<QueuedJob>> TryDequeue(CancellationToken cancellationToken)
    {
        var queuedJobs = await this.queuedJobService.GetJobsInPendingStatusByQueueType(this.QueueName, cancellationToken);

        return queuedJobs ?? [];
    }
}