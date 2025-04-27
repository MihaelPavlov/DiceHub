using DH.Domain.Adapters.Scheduling;
using DH.Domain.Adapters.Scheduling.Enums;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Repositories;

namespace DH.Adapter.Scheduling.Handlers;

/// <summary>
/// Implementation of <see cref="IReservationExpirationHandler"/>
/// </summary>
public class ReservationExpirationHandler : IReservationExpirationHandler
{
    readonly IRepository<GameReservation> gameReservationRepository;
    readonly IRepository<SpaceTableReservation> tableReservationRepository;
    readonly IRepository<GameInventory> inventoryRepository;
    readonly IRepository<FailedJob> failedJobsRepository;

    public ReservationExpirationHandler(IRepository<GameReservation> gameReservationRepository, IRepository<SpaceTableReservation> tableReservationRepository, IRepository<GameInventory> inventoryRepository, IRepository<FailedJob> failedJobsRepository)
    {
        this.gameReservationRepository = gameReservationRepository;
        this.tableReservationRepository = tableReservationRepository;
        this.failedJobsRepository = failedJobsRepository;
        this.inventoryRepository = inventoryRepository;
    }

    /// <inheritdoc/>
    public async Task ProcessFailedReservationExpirationAsync(string data, string errorMessage, CancellationToken cancellationToken)
    {
        await failedJobsRepository.AddAsync(new FailedJob { Data = data, Type = (int)JobType.ReservationExpiration, FailedAt = DateTime.UtcNow, ErrorMessage = errorMessage }, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task ProcessReservationExpirationAsync(CancellationToken cancellationToken)
    {
        var gameReservations = await gameReservationRepository.GetWithPropertiesAsync(x =>
            x.IsActive == true &&
            DateTime.UtcNow >= x.ReservationDate,
            x => x, cancellationToken);

        foreach (var reservation in gameReservations)
        {
            await ProcessGameReservationExpiration(reservation, cancellationToken);
        }

        var tableReservations = await tableReservationRepository.GetWithPropertiesAsync(x =>
            x.IsActive == true &&
            DateTime.UtcNow >= x.ReservationDate,
            x => x, cancellationToken);

        foreach (var reservation in tableReservations)
        {
            await ProcessTableReservationExpiration(reservation, cancellationToken);
        }

        await gameReservationRepository.SaveChangesAsync(cancellationToken);
    }

    private async Task ProcessGameReservationExpiration(GameReservation reservation, CancellationToken cancellationToken)
    {
        var inventory = await inventoryRepository.GetByAsyncWithTracking(x => x.GameId == reservation.GameId, cancellationToken);

        if (inventory is null)
            return;

        if (inventory.AvailableCopies < inventory.TotalCopies)
            inventory.AvailableCopies++;

        reservation.IsActive = false;
        reservation.IsReservationSuccessful = false;
        reservation.Status = ReservationStatus.Expired;

        await gameReservationRepository.Update(reservation, cancellationToken, false);
        await inventoryRepository.Update(inventory, cancellationToken, false);
    }

    private async Task ProcessTableReservationExpiration(SpaceTableReservation reservation, CancellationToken cancellationToken)
    {
        reservation.IsActive = false;
        reservation.IsReservationSuccessful = false;
        reservation.Status = ReservationStatus.Expired;

        await tableReservationRepository.Update(reservation, cancellationToken, false);
    }
}
