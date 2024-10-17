using Quartz.Listener;
using Quartz;
using DH.Domain.Adapters.Scheduling;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using DH.Adapter.Scheduling.Jobs;

namespace DH.Adapter.Scheduling;

/// <summary>
/// A job listener that listens for job execution results and processes any failures by adding them to a dead-letter queue.
/// </summary>
public class JobListenerForDeadLetterQueue : JobListenerSupport
{
    readonly IServiceScopeFactory serviceScopeFactory;

    public JobListenerForDeadLetterQueue(IServiceScopeFactory serviceScopeFactory)
    {
        this.serviceScopeFactory = serviceScopeFactory;
    }

    /// <summary>
    /// The name of the job listener.
    /// </summary>
    public override string Name => "JobListenerForDeadLetterQueue";

    /// <summary>
    /// Invoked after a job is executed. If the job fails, the failure details are processed and sent to a dead-letter queue.
    /// </summary>
    /// <param name="context">The context of the executed job.</param>
    /// <param name="jobException">The exception thrown during job execution, if any.</param>
    /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public override async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException? jobException, CancellationToken cancellationToken = default)
    {
        using (var scope = this.serviceScopeFactory.CreateScope())
        {
            if (jobException != null)
            {
                if (nameof(UserRewardsExpiryJob) == context.JobDetail.JobType.Name)
                {
                    var userRewardsExpiryHandler = scope.ServiceProvider.GetRequiredService<IUserRewardsExpiryHandler>();
                    await userRewardsExpiryHandler.ProcessFailedExpiryCheck(JsonSerializer.Serialize(new { context.JobDetail.Key, context.JobDetail.JobDataMap }), jobException.Message, cancellationToken);
                }
                else if (nameof(ExpireReservationJob) == context.JobDetail.JobType.Name)
                {
                    var reservationExpirationHandler = scope.ServiceProvider.GetRequiredService<IReservationExpirationHandler>();
                    await reservationExpirationHandler.ProcessFailedReservationExpirationAsync(JsonSerializer.Serialize(new { context.JobDetail.Key, context.JobDetail.JobDataMap }), jobException.Message, cancellationToken);
                }
            }
        }
    }
}
