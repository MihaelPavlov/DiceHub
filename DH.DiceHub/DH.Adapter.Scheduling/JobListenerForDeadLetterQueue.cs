using Quartz.Listener;
using Quartz;
using DH.Domain.Adapters.Scheduling;
using System.Text.Json;

namespace DH.Adapter.Scheduling;

public class JobListenerForDeadLetterQueue : JobListenerSupport
{
    readonly IReservationExpirationHandler reservationExpirationHandler;

    public JobListenerForDeadLetterQueue(IReservationExpirationHandler reservationExpirationHandler)
    {
        this.reservationExpirationHandler = reservationExpirationHandler;
    }

    public override string Name => "JobListenerForDeadLetterQueue";


    public override async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException? jobException, CancellationToken cancellationToken = default)
    {
        if (jobException != null)
        {
            await this.reservationExpirationHandler.ProcessFailedReservationExpirationAsync(JsonSerializer.Serialize(new { context.JobDetail.Key,context.JobDetail.JobDataMap }), jobException.Message, cancellationToken);
        }
    }
}
