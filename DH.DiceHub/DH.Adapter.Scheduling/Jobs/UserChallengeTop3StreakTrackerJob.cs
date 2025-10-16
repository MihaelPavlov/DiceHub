using DH.Domain.Services;
using Quartz;

namespace DH.Adapter.Scheduling.Jobs;

internal class UserChallengeTop3StreakTrackerJob(IUniversalChallengeProcessing universalChallengeProcessing) : IJob
{
    readonly IUniversalChallengeProcessing universalChallengeProcessing = universalChallengeProcessing;

    public Task Execute(IJobExecutionContext context)
    {
        throw new NotImplementedException();
    }
}
