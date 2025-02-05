using DH.Domain.Adapters.Reservations;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Repositories;
using DH.Domain.Services.Queue;
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
                using var scope = serviceScopeFactory.CreateScope();
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

                    await ProcessReservationJobAsync(scope, jobInfo, traceId, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    // Application is stopping, just ignore
                    logger.LogInformation("Job ID: {jobId} - Canceled at {cancelTime}.", traceId, DateTime.UtcNow);
                }
                catch (Exception ex)
                {
                    var queuedJobService = scope.ServiceProvider.GetRequiredService<IQueuedJobService>();
                    await queuedJobService.UpdateStatusToFailed(this.queue.QueueName, jobInfo.JobId);
                    logger.LogError(ex, "Job ID: {jobId} - Failed at {failureTime}: {jobInfo}", traceId, DateTime.UtcNow, JsonSerializer.Serialize(jobInfo));
                }
            }

            await Task.Delay(10000, cancellationToken);
        }
    }

    private async Task ProcessReservationJobAsync(IServiceScope scope, JobInfo jobInfo, string traceId, CancellationToken cancellationToken)
    {
        try
        {
            if (jobInfo.Type == ReservationType.Game)
            {
                var gameReservationRepository = scope.ServiceProvider.GetRequiredService<IRepository<GameReservation>>();
                var reservation = await gameReservationRepository.GetByAsyncWithTracking(x => x.Id == jobInfo.ReservationId, cancellationToken);

                if (reservation == null)
                {
                    logger.LogWarning("Job {traceId}: Reservation not found for id {reservationId}.", traceId, jobInfo.ReservationId);
                    return;
                }

                // Handle declined reservations immediately
                if (reservation.Status == ReservationStatus.Declined)
                {
                    await this.CleanupGameReservationAsync(scope, jobInfo.JobId, reservation, traceId, cancellationToken);
                    return;
                }

                // Handle time-based cleanup for non-declined reservations
                if (DateTime.UtcNow >= reservation.ReservationDate)
                {
                    if (reservation.IsActive)
                    {
                        await this.CleanupGameReservationAsync(scope, jobInfo.JobId, reservation, traceId, cancellationToken);
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
                await this.CleanupSpaceTableReservationAsync(scope, jobInfo, traceId, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process job {traceId}: {jobInfo}", traceId, JsonSerializer.Serialize(jobInfo));
            throw;
        }
    }

    private void RequeueJob(JobInfo jobInfo)
    {
        queue.RequeueJob(jobInfo);
        logger.LogInformation("Requeued job for reservation ID {reservationId}.", jobInfo.ReservationId);
    }

    private async Task CleanupGameReservationAsync(IServiceScope scope, Guid jobId, GameReservation reservation, string traceId, CancellationToken cancellationToken)
    {
        var gameInventoryRepository = scope.ServiceProvider.GetRequiredService<IRepository<GameInventory>>();
        var inventory = await gameInventoryRepository.GetByAsyncWithTracking(x => x.GameId == reservation.GameId, cancellationToken);

        if (inventory == null)
        {
            logger.LogWarning("Job {traceId}: Game inventory not found for gameId {gameId} and reservationId {reservationId}.", traceId, reservation.GameId, reservation.Id);
            return;
        }

        if (inventory.AvailableCopies < inventory.TotalCopies)
            inventory.AvailableCopies++;

        reservation.IsReservationSuccessful = false;
        reservation.IsActive = false;
        reservation.Status = ReservationStatus.Expired;

        await gameInventoryRepository.SaveChangesAsync(cancellationToken);

        var queuedJobService = scope.ServiceProvider.GetRequiredService<IQueuedJobService>();
        await queuedJobService.UpdateStatusToCompleted(this.queue.QueueName, jobId);

        logger.LogInformation("Job {traceId}: Deactivated reservation ID {reservationId}.", traceId, reservation.Id);
    }

    private async Task CleanupSpaceTableReservationAsync(IServiceScope scope, JobInfo jobInfo, string traceId, CancellationToken cancellationToken)
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
                reservation.Status = ReservationStatus.Expired;
                await repository.SaveChangesAsync(cancellationToken);

                var queuedJobService = scope.ServiceProvider.GetRequiredService<IQueuedJobService>();
                await queuedJobService.UpdateStatusToCompleted(this.queue.QueueName, jobInfo.JobId);

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
