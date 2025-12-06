using DH.Domain.Entities;
using DH.Domain.Queue;

namespace DH.Domain.Adapters.GameSession;

public interface IGameSessionQueue : QueueBase
{
    Task AddUserPlayTimEnforcerJob(string userId, int gameId, DateTime requiredPlayUntil);
    Task<bool> Contains(string userId, int gameId, CancellationToken cancellationToken);
    Task<List<QueuedJob>> TryDequeue(CancellationToken cancellationToken);
    Task CancelUserPlayTimeEnforcerJob(string userId, int gameId);
}
