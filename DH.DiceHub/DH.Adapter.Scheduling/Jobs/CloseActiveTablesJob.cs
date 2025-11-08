using DH.Domain.Services;
using DH.Domain.Services.TenantSettingsService;
using Quartz;

namespace DH.Adapter.Scheduling.Jobs;

public class CloseActiveTablesJob(ISpaceTableService spaceTablesService, ITenantSettingsCacheService tenantSettingsService) : IJob
{
    private readonly ISpaceTableService spaceTablesService = spaceTablesService;
    private readonly ITenantSettingsCacheService tenantSettingsService = tenantSettingsService;

    public async Task Execute(IJobExecutionContext context)
    {
        var tenantSettings = await this.tenantSettingsService.GetGlobalTenantSettingsAsync(context.CancellationToken);

        await this.spaceTablesService.CloseActiveTables(context.CancellationToken);
    }
}