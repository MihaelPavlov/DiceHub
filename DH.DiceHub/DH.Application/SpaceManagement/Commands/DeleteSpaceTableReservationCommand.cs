using DH.Domain.Adapters.Reservations;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;

namespace DH.Application.SpaceManagement.Commands;

public record DeleteSpaceTableReservationCommand(int Id) : IRequest;

internal class DeleteSpaceTableReservationCommandHandler(
    IRepository<SpaceTableReservation> repository,
    IReservationCleanupQueue queue) : IRequestHandler<DeleteSpaceTableReservationCommand>
{
    readonly IRepository<SpaceTableReservation> repository = repository;
    readonly IReservationCleanupQueue queue = queue;

    public async Task Handle(DeleteSpaceTableReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await this.repository.GetByAsyncWithTracking(x => x.Id == request.Id, cancellationToken)
           ?? throw new NotFoundException(nameof(SpaceTableReservation), request.Id);

        // Drop the pending cleanup job so ReservationCleanupWorker doesn't spin
        // on a reservation id that no longer exists.
        await this.queue.CancelReservationCleaningJob(reservation.Id, ReservationType.Table);

        await this.repository.Remove(reservation, cancellationToken);
    }
}
