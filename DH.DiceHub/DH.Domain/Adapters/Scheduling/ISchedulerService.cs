using DH.Domain.Adapters.Scheduling.Models;

namespace DH.Domain.Adapters.Scheduling;

public interface ISchedulerService
{
    Task ScheduleAddUserPeriodJob(CancellationToken cancellationToken);
    Task<List<ScheduleJobInfo>> GetScheduleJobs();
    Task<bool> DoesAddUserChallengePeriodJobExists();
    Task ScheduleCloseActiveTablesJob(CancellationToken cancellationToken);
}
