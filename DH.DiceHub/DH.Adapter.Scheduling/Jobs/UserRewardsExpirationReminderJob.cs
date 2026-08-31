using DH.Domain.Adapters.Scheduling;
using DH.OperationResultCore.Exceptions;
using Microsoft.Extensions.Logging;
using Quartz;

namespace DH.Adapter.Scheduling.Jobs;

/// <summary>
/// Sends "your reward expires in N days" reminders. Registered per tenant (job
/// key "UserRewardsExpirationReminderJob-{tenantId}", TenantId in the JobDataMap)
/// so the reminder window and the push hour match the user's local day. No
/// TenantId (manual test path) -> every tenant.
/// </summary>
[DisallowConcurrentExecution]
internal class UserRewardsExpirationReminderJob(
    IUserRewardsExpirationReminderHandler userRewardsExpirationReminderHandler,
    ILogger<UserRewardsExpirationReminderJob> logger) : IJob
{
    readonly IUserRewardsExpirationReminderHandler userRewardsExpirationReminderHandler = userRewardsExpirationReminderHandler;
    readonly ILogger<UserRewardsExpirationReminderJob> logger = logger;

    public async Task Execute(IJobExecutionContext context)
    {
        var cancellationToken = context?.CancellationToken ?? CancellationToken.None;
        var tenantId = context?.MergedJobDataMap.GetString("TenantId");

        try
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                await this.userRewardsExpirationReminderHandler.EvaluateRewardsReminder(cancellationToken);
            else
                await this.userRewardsExpirationReminderHandler.EvaluateRewardsReminder(tenantId, cancellationToken);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed during user rewards expiration reminder for {Tenant}.", tenantId ?? "<all tenants>");
            throw new InfrastructureException(ex.Message);
        }
    }
}
