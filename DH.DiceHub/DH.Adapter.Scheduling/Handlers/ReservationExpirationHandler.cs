using DH.Domain.Adapters.Scheduling;
using DH.Domain.Adapters.Scheduling.Enums;
using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Repositories;

namespace DH.Adapter.Scheduling.Handlers;

/// <summary>
/// Implementation of <see cref="IReservationExpirationHandler"/>
/// </summary>
public class ReservationExpirationHandler : IReservationExpirationHandler
{
    readonly IRepository<GameReservation> repository;
    readonly IRepository<GameInventory> inventoryRepository;
    readonly IRepository<FailedJob> failedJobsRepository;

    public ReservationExpirationHandler(IRepository<GameReservation> repository, IRepository<GameInventory> inventoryRepository, IRepository<FailedJob> failedJobsRepository)
    {
        this.repository = repository;
        this.failedJobsRepository = failedJobsRepository;
        this.inventoryRepository = inventoryRepository;
    }

    /// <inheritdoc/>
    public async Task ProcessFailedReservationExpirationAsync(string data, string errorMessage, CancellationToken cancellationToken)
    {
        await failedJobsRepository.AddAsync(new FailedJob { Data = data, Type = (int)JobType.GameReservationExpiration, FailedAt = DateTime.UtcNow, ErrorMessage = errorMessage }, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> ProcessReservationExpirationAsync(int reservationId, CancellationToken cancellationToken)
    {
        var reservation = await repository.GetByAsyncWithTracking(x => x.Id == reservationId, cancellationToken);

        if (reservation != null && reservation.IsActive && !reservation.IsReservationSuccessful)
        {
            var actualExpirationTime = reservation.ReservationDate.AddMinutes(reservation.ReservedDurationMinutes);

            if (DateTime.Now >= actualExpirationTime)
            {
                var inventory = await inventoryRepository.GetByAsyncWithTracking(x => x.GameId == reservation.GameId, cancellationToken)
                    ?? throw new NotFoundException(nameof(GameInventory));

                if (inventory.AvailableCopies < inventory.TotalCopies)
                    inventory.AvailableCopies++;

                reservation.IsActive = false;
                reservation.IsReservationSuccessful = false;

                await repository.Update(reservation, cancellationToken);
                await inventoryRepository.Update(inventory, cancellationToken);

                return true;
            }
        }

        return false;
    }
}
