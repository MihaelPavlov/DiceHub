using Quartz.Listener;
using Quartz;
using DH.Domain.Adapters.Scheduling;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace DH.Adapter.Scheduling;

public class JobListenerForDeadLetterQueue : JobListenerSupport
{
    readonly IServiceScopeFactory serviceScopeFactory;

    public JobListenerForDeadLetterQueue(IServiceScopeFactory serviceScopeFactory)
    {
        this.serviceScopeFactory = serviceScopeFactory;
    }

    public override string Name => "JobListenerForDeadLetterQueue";

    public override async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException? jobException, CancellationToken cancellationToken = default)
    {
        using (var scope = this.serviceScopeFactory.CreateScope())
        {
            if (jobException != null)
            {
                var reservationExpirationHandler = scope.ServiceProvider.GetRequiredService<IReservationExpirationHandler>();

                await reservationExpirationHandler.ProcessFailedReservationExpirationAsync(JsonSerializer.Serialize(new { context.JobDetail.Key, context.JobDetail.JobDataMap }), jobException.Message, cancellationToken);
            }
        }
    }
}
