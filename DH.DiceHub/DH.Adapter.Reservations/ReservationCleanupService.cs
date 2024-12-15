using DH.Domain.Adapters.Reservations;
using DH.Domain.Entities;
using DH.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using static DH.Domain.Adapters.Reservations.ReservationCleanupQueue;

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

                    if (DateTime.UtcNow < jobInfo.RemovingTime)
                    {
                        RequeueJob(jobInfo);
                        await Task.Delay(10000, cancellationToken); 
                        continue;
                    }

                    await ProcessReservationJobAsync(jobInfo, traceId, cancellationToken);
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

            await Task.Delay(10000, cancellationToken);
        }
    }

    private async Task ProcessReservationJobAsync(JobInfo jobInfo, string traceId, CancellationToken cancellationToken)
    {
        using var scope = serviceScopeFactory.CreateScope();
        try
        {
            if (jobInfo.Type == ReservationType.Game)
            {
                var repository = scope.ServiceProvider.GetRequiredService<IRepository<GameReservation>>();
                var reservation = await repository.GetByAsyncWithTracking(x => x.Id == jobInfo.ReservationId, cancellationToken);

                if (reservation == null)
                {
                    logger.LogWarning("Job {traceId}: Reservation not found for ID {reservationId}.", traceId, jobInfo.ReservationId);
                    return;
                }

                if (DateTime.UtcNow >= reservation.ReservationDate)
                {
                    if (reservation.IsActive)
                    {
                        reservation.IsActive = false;
                        reservation.IsReservationSuccessful = false;
                        await repository.SaveChangesAsync(cancellationToken);
                        logger.LogInformation("Job {traceId}: Deactivated reservation ID {reservationId}.", traceId, jobInfo.ReservationId);
                    }
                }
                else
                {
                    logger.LogInformation("Job {traceId}: Reservation ID {reservationId} not yet due for cleanup. Re-queuing.", traceId, jobInfo.ReservationId);
                    RequeueJob(jobInfo);
                }
            }
            else if (jobInfo.Type == ReservationType.Table)
            {
                var repository = scope.ServiceProvider.GetRequiredService<IRepository<SpaceTableReservation>>();
                var reservation = await repository.GetByAsyncWithTracking(x => x.Id == jobInfo.ReservationId, cancellationToken);

                if (reservation == null)
                {
                    logger.LogWarning("Job {traceId}: Reservation not found for ID {reservationId}.", traceId, jobInfo.ReservationId);
                    return;
                }

                if (DateTime.UtcNow >= reservation.ReservationDate)
                {
                    if (reservation.IsActive)
                    {
                        reservation.IsActive = false;
                        reservation.IsReservationSuccessful = false;
                        await repository.SaveChangesAsync(cancellationToken);
                        logger.LogInformation("Job {traceId}: Deactivated reservation ID {reservationId}.", traceId, jobInfo.ReservationId);
                    }
                }
                else
                {
                    logger.LogInformation("Job {traceId}: Reservation ID {reservationId} not yet due for cleanup. Re-queuing.", traceId, jobInfo.ReservationId);
                    RequeueJob(jobInfo);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process job {traceId}: {jobInfo}", traceId, JsonSerializer.Serialize(jobInfo));
        }
    }

    private void RequeueJob(JobInfo jobInfo)
    {
        queue.AddReservationCleaningJob(jobInfo.ReservationId, jobInfo.Type, jobInfo.RemovingTime);
        logger.LogInformation("Requeued job for reservation ID {reservationId}.", jobInfo.ReservationId);
    }
}
