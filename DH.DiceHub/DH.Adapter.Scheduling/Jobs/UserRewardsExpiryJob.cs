using DH.Domain.Adapters.Scheduling;
using DH.OperationResultCore.Exceptions;
using Quartz;

namespace DH.Adapter.Scheduling.Jobs;

internal class UserRewardsExpiryJob : IJob
{
    readonly IUserRewardsExpiryHandler rewardsExpiryHandler;

    public UserRewardsExpiryJob(IUserRewardsExpiryHandler rewardsExpiryHandler)
    {
        this.rewardsExpiryHandler = rewardsExpiryHandler;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            await this.rewardsExpiryHandler.EvaluateUserRewardsExpiry(CancellationToken.None);
        }
        catch (Exception ex)
        {
            throw new InfrastructureException(ex.Message);
        }
    }
}
