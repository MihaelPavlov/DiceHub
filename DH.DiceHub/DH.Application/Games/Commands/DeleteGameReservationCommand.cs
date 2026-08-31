using DH.Domain.Adapters.Reservations;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;

namespace DH.Application.Games.Commands;

public record DeleteGameReservationCommand(int Id) : IRequest;

internal class DeleteGameReservationCommandHandler(
    IRepository<GameReservation> repository,
    IRepository<GameInventory> gameInventoryRepository,
    IReservationCleanupQueue queue) : IRequestHandler<DeleteGameReservationCommand>
{
    readonly IRepository<GameReservation> repository = repository;
    readonly IRepository<GameInventory> gameInventoryRepository = gameInventoryRepository;
    readonly IReservationCleanupQueue queue = queue;

    public async Task Handle(DeleteGameReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await this.repository.GetByAsyncWithTracking(x => x.Id == request.Id, cancellationToken)
           ?? throw new NotFoundException(nameof(GameReservation), request.Id);

        // A live reservation still holds a game copy: CreateReservation decrements
        // AvailableCopies and it is only returned by cancel/decline/expiry/cleanup
        // (all of which flip IsActive to false). Deleting the row outright skips
        // that, permanently leaking the copy - so return it here first.
        if (reservation.IsActive)
        {
            var inventory = await this.gameInventoryRepository
                .GetByAsyncWithTracking(x => x.GameId == reservation.GameId, cancellationToken);

            if (inventory != null && inventory.AvailableCopies < inventory.TotalCopies)
            {
                inventory.AvailableCopies++;
                await this.gameInventoryRepository.SaveChangesAsync(cancellationToken);
            }
        }

        // Drop the pending cleanup job so ReservationCleanupWorker doesn't spin
        // on a reservation id that no longer exists.
        await this.queue.CancelReservationCleaningJob(reservation.Id, ReservationType.Game);

        await this.repository.Remove(reservation, cancellationToken);
    }
}
