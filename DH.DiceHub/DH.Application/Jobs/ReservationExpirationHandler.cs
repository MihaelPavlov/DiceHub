using DH.Domain.Adapters.Scheduling;
using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Repositories;

namespace DH.Application.Jobs;

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

    public async Task ProcessFailedReservationExpirationAsync(string data, string errorMessage, CancellationToken cancellationToken)
    {
        await this.failedJobsRepository.AddAsync(new FailedJob { Data = data, Type = 0, FailedAt = DateTime.UtcNow, ErrorMessage = errorMessage }, cancellationToken);
    }

    public async Task<bool> ProcessReservationExpirationAsync(int reservationId, CancellationToken cancellationToken)
    {
        var reservation = await this.repository.GetByAsyncWithTracking(x => x.Id == reservationId, cancellationToken);

        if (reservation != null && !reservation.IsExpired)
        {
            var actualExpirationTime = reservation.ReservationDate.AddMinutes(reservation.ReservedDurationMinutes);

            if (DateTime.Now >= actualExpirationTime)
            {
                var inventory = await this.inventoryRepository.GetByAsyncWithTracking(x => x.GameId == reservation.GameId, cancellationToken)
                    ?? throw new NotFoundException(nameof(GameInventory));

                if (inventory.AvailableCopies < inventory.TotalCopies)
                    inventory.AvailableCopies++;

                reservation.IsExpired = true;

                await this.repository.Update(reservation, cancellationToken);
                await this.inventoryRepository.Update(inventory, cancellationToken);

                return true;
            }
        }

        return false;
    }
}
