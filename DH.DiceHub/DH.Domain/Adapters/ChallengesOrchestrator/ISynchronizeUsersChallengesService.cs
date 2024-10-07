using static DH.Domain.Adapters.ChallengesOrchestrator.SynchronizeUsersChallengesQueue;

namespace DH.Domain.Adapters.ChallengesOrchestrator;

public interface ISynchronizeUsersChallengesService
{
   Task SynchronizeNewUserJob(SynchronizeNewUserJob job, CancellationToken cancellationToken);
   Task ChallengeInitiationJob(ChallengeInitiationJob job, CancellationToken cancellationToken);
}
