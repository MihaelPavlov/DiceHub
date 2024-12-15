using DH.Domain.Adapters.Reservations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace DH.Adapter.Reservations;

public class ReservationCleanupService : BackgroundService
{
    readonly ILogger<ReservationCleanupService> logger;
    readonly ReservationCleanupQueue queue;
    readonly IServiceScopeFactory serviceScopeFactory;

    public ReservationCleanupService(IServiceScopeFactory serviceScopeFactory, ILogger<ReservationCleanupService> logger, ReservationCleanupQueue queue)
    {
        this.serviceScopeFactory = serviceScopeFactory;
        this.logger = logger;
        this.queue = queue;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {

        while (!cancellationToken.IsCancellationRequested)
        {
            if (this.queue.TryDequeue(out var jobInfo))
            {
                string traceId = Guid.NewGuid().ToString();
                try
                {
                    var jobStartTime = DateTime.UtcNow;
                    logger.LogInformation("Job ID: {jobId} - Started at {startTime} - Job Info: {jobInfo}", traceId, jobStartTime, JsonSerializer.Serialize(jobInfo));

                    if (DateTime.UtcNow >= jobInfo.RemovingTime)
                    {
                        /*
                         Get the reservation based on the reservation Type
                        0 GameReservation
                        1 SpaceTableReservation
                        After check the status

                        Approved
                            Check again if 15 or more minutes are passed from the reservation time and the reservation is still isReservationSucccesuflly = false;
                            If the 15 minutes are passed, update the reservation isActive= false and saveChanges;
                        Declined
                            Check if 10 minutes after the decline is passed.
                            This might need to add UpdateTime prop to the Reservations Models
                            If it's passed update the reservation isActive = false

                        Notes: 
                            We can add the additional time after the reservation time in tenantSettings,
                         */

                        continue;
                    }

                    // Add to the queue the job again if it's not completed
                }
                catch (TaskCanceledException)
                {
                    // Application is stopping, just ignore
                    logger.LogInformation("Job ID: {jobId} - Canceled at {cancelTime}.", traceId, DateTime.UtcNow);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Job ID: {jobId} - Failed at {failureTime}: {jobInfo}", traceId, DateTime.UtcNow, JsonSerializer.Serialize(jobInfo));
                }
            }
        }
    }
}
