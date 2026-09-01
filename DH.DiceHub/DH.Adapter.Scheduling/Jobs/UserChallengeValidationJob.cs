using DH.Domain.Services;
using Microsoft.Extensions.Logging;
using Quartz;

namespace DH.Adapter.Scheduling.Jobs;

/// <summary>
/// Validates user challenge periods. Registered per tenant (job key
/// "UserChallengeValidationJob-{tenantId}", TenantId in the JobDataMap) so it
/// fires at each tenant's configured local time. If no TenantId is present
/// (e.g. the manual test endpoint), it falls back to processing every tenant.
/// </summary>
[DisallowConcurrentExecution]
public class UserChallengeValidationJob : IJob
{
    readonly IUserChallengesManagementService userChallengesManagementService;
    readonly ILogger<UserChallengeValidationJob> logger;

    public UserChallengeValidationJob(
        IUserChallengesManagementService userChallengesManagementService,
        ILogger<UserChallengeValidationJob> logger)
    {
        this.userChallengesManagementService = userChallengesManagementService;
        this.logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var cancellationToken = context?.CancellationToken ?? CancellationToken.None;
        var tenantId = context?.MergedJobDataMap.GetString("TenantId");

        try
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                await this.userChallengesManagementService.EnsureValidUserChallengePeriodsAsync(cancellationToken);
            else
                await this.userChallengesManagementService.EnsureValidUserChallengePeriodsAsync(tenantId, cancellationToken);

            this.logger.LogInformation("User Challenge Period Validation check completed for {Tenant}.", tenantId ?? "<all tenants>");
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed during User Challenge Period Validation for {Tenant}.", tenantId ?? "<all tenants>");
        }
    }
}
