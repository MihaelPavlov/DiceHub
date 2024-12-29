using DH.Domain.Adapters.Scheduling;
using DH.OperationResultCore.Exceptions;
using Quartz;

namespace DH.Adapter.Scheduling.Jobs;

internal class UserRewardsExpirationReminderJob : IJob
{
    readonly IUserRewardsExpirationReminderHandler userRewardsExpirationReminderHandler;

    public UserRewardsExpirationReminderJob(IUserRewardsExpirationReminderHandler userRewardsExpirationReminderHandler)
    {
        this.userRewardsExpirationReminderHandler = userRewardsExpirationReminderHandler;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            await this.userRewardsExpirationReminderHandler.EvaluateRewardsReminder(CancellationToken.None);
        }
        catch (Exception ex)
        {
            throw new InfrastructureException(ex.Message);
        }
    }
}
