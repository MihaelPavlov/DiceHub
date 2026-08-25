using DH.Domain.Services;
using Quartz;

namespace DH.Adapter.Scheduling.Jobs;

public class CloseActiveTablesJob(ISpaceTableService spaceTablesService, ITenantContextScopeRunner tenantContextScopeRunner) : IJob
{
    private readonly ISpaceTableService spaceTablesService = spaceTablesService;
    private readonly ITenantContextScopeRunner tenantContextScopeRunner = tenantContextScopeRunner;

    public async Task Execute(IJobExecutionContext context)
    {
        var tenantId = context.MergedJobDataMap.GetString("TenantId");
        if (string.IsNullOrWhiteSpace(tenantId))
            throw new InvalidOperationException($"{nameof(CloseActiveTablesJob)} trigger {context.Trigger.Key} has no TenantId in its JobDataMap.");

        await this.tenantContextScopeRunner.RunAsTenantAsync(tenantId,
            () => this.spaceTablesService.CloseActiveTables(context.CancellationToken));
    }
}