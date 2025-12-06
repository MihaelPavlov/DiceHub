using DH.Domain.Entities;
using DH.Domain.Queue;

namespace DH.Domain.Adapters.ChallengesOrchestrator;

public interface ISynchronizeUsersChallengesQueue : QueueBase
{
    Task AddSynchronizeNewUserJob(string userId);
    Task AddChallengeInitiationJob(string userId, DateTime scheduledTime);
    Task<List<QueuedJob>> TryDequeue(CancellationToken cancellationToken);
}
