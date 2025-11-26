using DH.Domain.Services;
using Quartz;

namespace DH.Adapter.Scheduling.Jobs;

public class CloseActiveTablesJob(ISpaceTableService spaceTablesService) : IJob
{
    private readonly ISpaceTableService spaceTablesService = spaceTablesService;

    public async Task Execute(IJobExecutionContext context)
    {
        await this.spaceTablesService.CloseActiveTables(context.CancellationToken);
    }
}