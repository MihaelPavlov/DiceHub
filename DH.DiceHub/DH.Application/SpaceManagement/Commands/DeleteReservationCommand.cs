using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.SpaceManagement.Commands;

public record DeleteReservationCommand(int Id) : IRequest;

internal class DeleteReservationCommandHandler(IRepository<SpaceTableReservation> repository) : IRequestHandler<DeleteReservationCommand>
{
    readonly IRepository<SpaceTableReservation> repository = repository;

    public async Task Handle(DeleteReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await this.repository.GetByAsyncWithTracking(x => x.Id == request.Id, cancellationToken)
           ?? throw new NotFoundException(nameof(SpaceTableReservation), request.Id);

        await this.repository.Remove(reservation, cancellationToken);
    }
}
