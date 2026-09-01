using DH.Domain.Services;
using DH.OperationResultCore.Exceptions;
using Microsoft.Extensions.Logging;
using Quartz;

namespace DH.Adapter.Scheduling.Jobs;

/// <summary>
/// Tracks the "stay in the top 3" streak challenge. Registered per tenant (job
/// key "UserChallengeTop3StreakTrackerJob-{tenantId}", TenantId in the
/// JobDataMap) so it fires at each tenant's configured local time. If no
/// TenantId is present it falls back to processing every tenant.
/// </summary>
[DisallowConcurrentExecution]
internal class UserChallengeTop3StreakTrackerJob(
    IUniversalChallengeProcessing universalChallengeProcessing,
    ILogger<UserChallengeTop3StreakTrackerJob> logger) : IJob
{
    readonly IUniversalChallengeProcessing universalChallengeProcessing = universalChallengeProcessing;
    readonly ILogger<UserChallengeTop3StreakTrackerJob> logger = logger;

    public async Task Execute(IJobExecutionContext context)
    {
        var cancellationToken = context?.CancellationToken ?? CancellationToken.None;
        var tenantId = context?.MergedJobDataMap.GetString("TenantId");

        try
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                await this.universalChallengeProcessing.ProcessUserChallengeTop3Streak(cancellationToken);
            else
                await this.universalChallengeProcessing.ProcessUserChallengeTop3Streak(tenantId, cancellationToken);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed during User Challenge Top 3 Streak tracking for {Tenant}.", tenantId ?? "<all tenants>");
            throw new InfrastructureException(ex.Message);
        }
    }
}
