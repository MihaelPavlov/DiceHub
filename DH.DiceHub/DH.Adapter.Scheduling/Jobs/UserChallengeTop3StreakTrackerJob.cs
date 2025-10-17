using DH.Domain.Services;
using DH.OperationResultCore.Exceptions;
using Quartz;

namespace DH.Adapter.Scheduling.Jobs;

internal class UserChallengeTop3StreakTrackerJob(IUniversalChallengeProcessing universalChallengeProcessing) : IJob
{
    readonly IUniversalChallengeProcessing universalChallengeProcessing = universalChallengeProcessing;

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            await this.universalChallengeProcessing.ProcessUserChallengeTop3Streak(CancellationToken.None);
        }
        catch (Exception ex)
        {
            throw new InfrastructureException(ex.Message);
        }
    }
}
