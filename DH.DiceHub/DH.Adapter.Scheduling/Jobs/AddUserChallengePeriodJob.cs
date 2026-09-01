using DH.Domain.Adapters.Scheduling;
using DH.Domain.Entities;
using DH.Domain.Repositories;
using DH.Domain.Services;
using DH.Domain.Services.TenantSettingsService;
using DH.OperationResultCore.Exceptions;
using Microsoft.Extensions.Logging;
using Quartz;

namespace DH.Adapter.Scheduling.Jobs;

/// <summary>
/// Creates the next challenge-period performance rows for a tenant's users, then
/// re-arms its own one-shot trigger for that tenant's following reset date.
/// Registered per tenant (job key "AddUserChallengePeriodJob-{tenantId}",
/// TenantId in the JobDataMap) so the reset lands at the tenant's local time in
/// its own zone. No TenantId (manual test path) -> every tenant, no re-arm.
/// </summary>
[DisallowConcurrentExecution]
public class AddUserChallengePeriodJob : IJob
{
    readonly IAddUserChallengePeriodHandler addUserChallengePeriodHandler;
    readonly ISchedulerService schedulerService;
    readonly ITenantSettingsCacheService tenantSettingsService;
    readonly ITenantContextScopeRunner tenantContextScopeRunner;
    readonly IRepository<TenantSetting> repository;
    readonly ILogger<AddUserChallengePeriodJob> logger;

    public AddUserChallengePeriodJob(
        IAddUserChallengePeriodHandler addUserChallengePeriodHandler,
        ISchedulerService schedulerService,
        ITenantSettingsCacheService tenantSettingsService,
        ITenantContextScopeRunner tenantContextScopeRunner,
        IRepository<TenantSetting> repository,
        ILogger<AddUserChallengePeriodJob> logger)
    {
        this.addUserChallengePeriodHandler = addUserChallengePeriodHandler;
        this.schedulerService = schedulerService;
        this.tenantSettingsService = tenantSettingsService;
        this.tenantContextScopeRunner = tenantContextScopeRunner;
        this.repository = repository;
        this.logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var cancellationToken = context?.CancellationToken ?? CancellationToken.None;
        var tenantId = context?.MergedJobDataMap.GetString("TenantId");

        this.logger.LogInformation(
            "AddUserChallengePeriodJob started for {Tenant} at {RunAt}.", tenantId ?? "<all tenants>", DateTime.UtcNow);

        // Manual test path: run for every tenant, never re-arm, and let the caller
        // see the exception.
        if (string.IsNullOrWhiteSpace(tenantId))
        {
            try
            {
                await this.addUserChallengePeriodHandler.InitializeNewPeriods(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new InfrastructureException(ex.Message);
            }
            return;
        }

        // IMPORTANT: this is a one-shot, self-rescheduling trigger. If Execute throws,
        // Quartz parks the trigger in ERROR state and it never fires again - which
        // silently freezes the whole tenant's reward periods (exactly what happened
        // to smini-dragon). So: run the two steps independently, swallow failures
        // after logging, and ALWAYS try to re-arm the next trigger.
        await this.tenantContextScopeRunner.RunAsTenantAsync(tenantId, async () =>
        {
            try
            {
                await this.addUserChallengePeriodHandler.InitializeNewPeriods(tenantId, cancellationToken);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex,
                    "AddUserChallengePeriodJob: InitializeNewPeriods failed for tenant {TenantId}. Re-arming the trigger anyway.",
                    tenantId);
            }

            try
            {
                var tenantSettings = await this.tenantSettingsService.GetGlobalTenantSettingsAsync(cancellationToken);

                // Re-arm this tenant's own trigger for the next reset. Explicit-params
                // overload: we already hold the settings and the ambient tenant scope,
                // so it must not open another RunAsTenantAsync.
                var nextRunAt = await this.schedulerService.ScheduleAddUserPeriodJobForTenant(
                    tenantId,
                    tenantSettings.PeriodOfRewardReset,
                    tenantSettings.ResetDayForRewards,
                    tenantSettings.TimeZoneId,
                    replaceExisting: true,
                    cancellationToken);

                if (nextRunAt.HasValue)
                {
                    var dbSettings = await this.repository.GetByAsyncWithTracking(x => x.Id == tenantSettings.Id, cancellationToken);
                    if (dbSettings != null)
                    {
                        dbSettings.NextResetTimeOfPeriod = nextRunAt.Value.ToUniversalTime();
                        await this.repository.SaveChangesAsync(cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex,
                    "AddUserChallengePeriodJob: failed to re-arm the next trigger for tenant {TenantId}. The startup reconciler will recover it.",
                    tenantId);
            }
        });

        this.logger.LogInformation("AddUserChallengePeriodJob completed for tenant {TenantId}.", tenantId);
    }
}
