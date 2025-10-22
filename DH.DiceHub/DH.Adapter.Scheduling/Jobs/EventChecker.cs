using DH.Domain.Services;
using DH.OperationResultCore.Exceptions;
using Quartz;

namespace DH.Adapter.Scheduling.Jobs;

public class EventChecker(IUniversalChallengeProcessing universalChallengeProcessing) : IJob
{
    readonly IUniversalChallengeProcessing universalChallengeProcessing = universalChallengeProcessing;

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            await this.universalChallengeProcessing.ProcessJoinXEventsChallenge(CancellationToken.None);
        }
        catch (Exception ex)
        {
            throw new InfrastructureException(ex.Message);
        }
    }
}
