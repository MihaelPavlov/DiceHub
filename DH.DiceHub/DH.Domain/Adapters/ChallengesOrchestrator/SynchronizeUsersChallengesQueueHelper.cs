
namespace DH.Domain.Adapters.ChallengesOrchestrator;

public static class SynchronizeUsersChallengesQueueHelper
{
    public static string BuildJobId(string typeOfJob, string userId) => $"{typeOfJob}-{userId}";
}
