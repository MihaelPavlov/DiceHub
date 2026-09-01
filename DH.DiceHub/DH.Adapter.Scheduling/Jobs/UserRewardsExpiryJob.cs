using DH.Domain.Adapters.Scheduling;
using DH.OperationResultCore.Exceptions;
using Microsoft.Extensions.Logging;
using Quartz;

namespace DH.Adapter.Scheduling.Jobs;

/// <summary>
/// Marks expired user rewards. Registered per tenant (job key
/// "UserRewardsExpiryJob-{tenantId}", TenantId in the JobDataMap) so it fires at
/// each tenant's local midnight. No TenantId (manual test path) -> every tenant.
/// </summary>
[DisallowConcurrentExecution]
internal class UserRewardsExpiryJob(
    IUserRewardsExpiryHandler rewardsExpiryHandler,
    ILogger<UserRewardsExpiryJob> logger) : IJob
{
    readonly IUserRewardsExpiryHandler rewardsExpiryHandler = rewardsExpiryHandler;
    readonly ILogger<UserRewardsExpiryJob> logger = logger;

    public async Task Execute(IJobExecutionContext context)
    {
        var cancellationToken = context?.CancellationToken ?? CancellationToken.None;
        var tenantId = context?.MergedJobDataMap.GetString("TenantId");

        try
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                await this.rewardsExpiryHandler.EvaluateUserRewardsExpiry(cancellationToken);
            else
                await this.rewardsExpiryHandler.EvaluateUserRewardsExpiry(tenantId, cancellationToken);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed during user rewards expiry for {Tenant}.", tenantId ?? "<all tenants>");
            throw new InfrastructureException(ex.Message);
        }
    }
}
