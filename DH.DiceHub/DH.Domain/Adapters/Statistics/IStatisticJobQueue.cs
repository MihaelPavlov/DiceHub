using DH.Domain.Entities;
using DH.Domain.Queue;

namespace DH.Domain.Adapters.Statistics;

public interface IStatisticJobQueue : QueueBase
{
    Task<List<QueuedJob>> TryDequeue(CancellationToken cancellationToken);
}
