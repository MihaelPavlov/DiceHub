using DH.Domain.Adapters.Reservations;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Repositories;
using DH.Domain.Services.Queue;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace DH.Adapter.Reservations;

public class ReservationCleanupWorker : BackgroundService
{
    readonly ILogger<ReservationCleanupWorker> logger;
    readonly IServiceScopeFactory serviceScopeFactory;

    public ReservationCleanupWorker(IServiceScopeFactory serviceScopeFactory, ILogger<ReservationCleanupWorker> logger)
    {
        this.serviceScopeFactory = serviceScopeFactory;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var queue = scope.ServiceProvider.GetRequiredService<IReservationCleanupQueue>();

            var queuedJobs = await queue.TryDequeue(cancellationToken);

            var nextJobsForProcessing = queuedJobs
                .Select(q => JsonSerializer.Deserialize<ReservationCleanupJobInfo>(q.MessagePayload)!)
                .Where(x => DateTime.UtcNow > x.RemovingTime);

            foreach (var nextJob in nextJobsForProcessing)
            {
                string traceId = Guid.NewGuid().ToString();
                try
                {
                    var jobStartTime = DateTime.UtcNow;
                    logger.LogInformation("Job ID: {jobId} - Started at {startTime} - Job Info: {jobInfo}", traceId, jobStartTime, JsonSerializer.Serialize(nextJob));

                    await ProcessReservationJobAsync(scope, nextJob, traceId, queue.QueueName, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    // Application is stopping, just ignore
                    logger.LogInformation("Job ID: {jobId} - Canceled at {cancelTime}.", traceId, DateTime.UtcNow);
                }
                catch (Exception ex)
                {
                    var queuedJobService = scope.ServiceProvider.GetRequiredService<IQueuedJobService>();
                    await queuedJobService.UpdateStatusToFailed(queue.QueueName, nextJob.JobId);
                    logger.LogError(ex, "Job ID: {jobId} - Failed at {failureTime}: {jobInfo}", traceId, DateTime.UtcNow, JsonSerializer.Serialize(nextJob));
                }
            }

            if (!nextJobsForProcessing.Any())
                await Task.Delay(TimeSpan.FromMinutes(3), cancellationToken);
        }
    }

    private async Task ProcessReservationJobAsync(IServiceScope scope, ReservationCleanupJobInfo jobInfo, string traceId, string queueName, CancellationToken cancellationToken)
    {
        try
        {
            if (jobInfo.Type == ReservationType.Game)
            {
                var gameReservationRepository = scope.ServiceProvider.GetRequiredService<IRepository<GameReservation>>();
                var reservation = await gameReservationRepository.GetByAsyncWithTracking(x => x.Id == jobInfo.ReservationId, cancellationToken);

                if (reservation == null)
                {
                    logger.LogWarning("Job {traceId}: Game Reservation not found for id {reservationId}.", traceId, jobInfo.ReservationId);
                    return;
                }

                // Handle declined reservations immediately
                if (reservation.Status == ReservationStatus.Declined)
                {
                    await this.CleanupGameReservationAsync(scope, jobInfo.JobId, reservation, traceId, cancellationToken, queueName, isDeclined: true);
                    return;
                }

                if (reservation.Status == ReservationStatus.Expired)
                {
                    await this.CleanupGameReservationAsync(scope, jobInfo.JobId, reservation, traceId, cancellationToken, queueName);
                    return;
                }

                // Handle time-based cleanup for non-declined reservations
                if (DateTime.UtcNow >= reservation.ReservationDate && reservation.Status == ReservationStatus.Pending)
                {
                    if (reservation.IsActive)
                    {
                        await this.CleanupGameReservationAsync(scope, jobInfo.JobId, reservation, traceId, cancellationToken, queueName);
                    }
                    else
                    {
                        var queuedJobService = scope.ServiceProvider.GetRequiredService<IQueuedJobService>();
                        await queuedJobService.UpdateStatusToCompleted(queueName, jobInfo.JobId);
                    }
                }
                else if (DateTime.UtcNow >= reservation.ReservationDate && reservation.Status == ReservationStatus.Accepted)
                {
                    if (reservation.IsActive)
                    {
                        await this.CleanupGameReservationAsync(scope, jobInfo.JobId, reservation, traceId, cancellationToken, queueName, isApproved: true);
                    }
                    else
                    {
                        var queuedJobService = scope.ServiceProvider.GetRequiredService<IQueuedJobService>();
                        await queuedJobService.UpdateStatusToCompleted(queueName, jobInfo.JobId);
                    }
                }
                else
                {
                    logger.LogInformation("Job {traceId}: Game Reservation ID {reservationId} did not meet the necessary processing conditions", traceId, jobInfo.ReservationId);
                }
            }
            else if (jobInfo.Type == ReservationType.Table)
            {
                var spaceTableReservationRepo = scope.ServiceProvider.GetRequiredService<IRepository<SpaceTableReservation>>();
                var reservation = await spaceTableReservationRepo.GetByAsyncWithTracking(x => x.Id == jobInfo.ReservationId, cancellationToken);

                if (reservation == null)
                {
                    logger.LogWarning("Job {traceId}: Table Reservation not found for id {reservationId}.", traceId, jobInfo.ReservationId);
                    return;
                }

                // Handle declined reservations immediately
                if (reservation.Status == ReservationStatus.Declined)
                {
                    await this.CleanupSpaceTableReservationAsync(scope, spaceTableReservationRepo,
                        jobInfo.JobId, reservation, traceId, queueName, cancellationToken, isDeclined: true);
                    return;
                }

                if (reservation.Status == ReservationStatus.Expired)
                {
                    await this.CleanupSpaceTableReservationAsync(scope, spaceTableReservationRepo,
                        jobInfo.JobId, reservation, traceId, queueName, cancellationToken);
                    return;
                }

                // Handle time-based cleanup for non-declined reservations
                if (DateTime.UtcNow >= reservation.ReservationDate && reservation.Status == ReservationStatus.Pending)
                {
                    if (reservation.IsActive)
                    {
                        await this.CleanupSpaceTableReservationAsync(scope, spaceTableReservationRepo,
                            jobInfo.JobId, reservation, traceId, queueName, cancellationToken);
                    }
                    else
                    {
                        var queuedJobService = scope.ServiceProvider.GetRequiredService<IQueuedJobService>();
                        await queuedJobService.UpdateStatusToCompleted(queueName, jobInfo.JobId);
                    }
                }
                else if (DateTime.UtcNow >= reservation.ReservationDate && reservation.Status == ReservationStatus.Accepted)
                {
                    if (reservation.IsActive)
                    {
                        await this.CleanupSpaceTableReservationAsync(scope, spaceTableReservationRepo,
                            jobInfo.JobId, reservation, traceId, queueName, cancellationToken, isApproved: true);
                    }
                    else
                    {
                        var queuedJobService = scope.ServiceProvider.GetRequiredService<IQueuedJobService>();
                        await queuedJobService.UpdateStatusToCompleted(queueName, jobInfo.JobId);
                    }
                }
                else
                {
                    logger.LogInformation("Job {traceId}: Table Reservation ID {reservationId} did not meet the necessary processing conditions", traceId, jobInfo.ReservationId);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process job {traceId}: {jobInfo}", traceId, JsonSerializer.Serialize(jobInfo));
            throw;
        }
    }

    private async Task CleanupGameReservationAsync(
        IServiceScope scope, string jobId, GameReservation reservation,
        string traceId, CancellationToken cancellationToken, string queueName, bool isDeclined = false, bool isApproved = false)
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

        reservation.IsActive = false;

        if (!isApproved)
        {
            reservation.IsReservationSuccessful = false;
            reservation.Status = isDeclined ? ReservationStatus.Declined : ReservationStatus.Expired;
        }
        await gameInventoryRepository.SaveChangesAsync(cancellationToken);

        var queuedJobService = scope.ServiceProvider.GetRequiredService<IQueuedJobService>();
        await queuedJobService.UpdateStatusToCompleted(queueName, jobId);

        logger.LogInformation("Job {traceId}: Game reservation ID {reservationId}. Cleaning job was processed", traceId, reservation.Id);
    }

    private async Task CleanupSpaceTableReservationAsync(
        IServiceScope scope, IRepository<SpaceTableReservation> repository, string jobId,
        SpaceTableReservation reservation, string traceId, string queueName, CancellationToken cancellationToken, bool isDeclined = false, bool isApproved = false)
    {
        reservation.IsActive = false;
        if (!isApproved)
        {
            reservation.IsReservationSuccessful = false;
            reservation.Status = isDeclined ? ReservationStatus.Declined : ReservationStatus.Expired;
        }
        await repository.SaveChangesAsync(cancellationToken);

        var queuedJobService = scope.ServiceProvider.GetRequiredService<IQueuedJobService>();
        await queuedJobService.UpdateStatusToCompleted(queueName, jobId);

        logger.LogInformation("Job {traceId}: Table reservation ID {reservationId}. Cleaning job was processed.", traceId, reservation.Id);
    }
}
