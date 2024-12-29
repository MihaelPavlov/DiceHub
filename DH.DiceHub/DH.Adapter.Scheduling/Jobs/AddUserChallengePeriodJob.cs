using DH.Domain.Adapters.Scheduling;
using DH.OperationResultCore.Exceptions;
using Quartz;

namespace DH.Adapter.Scheduling.Jobs;

public class AddUserChallengePeriodJob : IJob
{
    readonly IAddUserChallengePeriodHandler addUserChallengePeriodHandler;

    public AddUserChallengePeriodJob(IAddUserChallengePeriodHandler addUserChallengePeriodHandler)
    {
        this.addUserChallengePeriodHandler = addUserChallengePeriodHandler;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            await this.addUserChallengePeriodHandler.InitializeNewPeriod(CancellationToken.None);
        }
        catch (Exception ex)
        {
            throw new InfrastructureException(ex.Message);
        }
    }
}
