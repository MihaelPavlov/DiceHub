using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Games.Commands;

public record DeleteGameReservationCommand(int Id) : IRequest;

internal class DeleteGameReservationCommandHandler(IRepository<GameReservation> repository) : IRequestHandler<DeleteGameReservationCommand>
{
    readonly IRepository<GameReservation> repository = repository;

    public async Task Handle(DeleteGameReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await this.repository.GetByAsyncWithTracking(x => x.Id == request.Id, cancellationToken)
           ?? throw new NotFoundException(nameof(GameReservation), request.Id);

        await this.repository.Remove(reservation, cancellationToken);
    }
}
