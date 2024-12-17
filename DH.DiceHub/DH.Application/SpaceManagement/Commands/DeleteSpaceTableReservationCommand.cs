using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.SpaceManagement.Commands;

public record DeleteSpaceTableReservationCommand(int Id) : IRequest;

internal class DeleteSpaceTableReservationCommandHandler(IRepository<SpaceTableReservation> repository) : IRequestHandler<DeleteSpaceTableReservationCommand>
{
    readonly IRepository<SpaceTableReservation> repository = repository;

    public async Task Handle(DeleteSpaceTableReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await this.repository.GetByAsyncWithTracking(x => x.Id == request.Id, cancellationToken)
           ?? throw new NotFoundException(nameof(SpaceTableReservation), request.Id);

        await this.repository.Remove(reservation, cancellationToken);
    }
}
