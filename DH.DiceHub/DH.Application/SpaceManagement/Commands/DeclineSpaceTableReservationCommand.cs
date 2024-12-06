using DH.Domain.Adapters.Authentication.Models;
using DH.Domain.Adapters.PushNotifications.Messages;
using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Exceptions;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.SpaceManagement.Commands;

public record DeclineSpaceTableReservationCommand(int ReservationId, string InternalNote, string PublicNote) : IRequest;

internal class DeclineSpaceTableReservationCommandHandler(IRepository<SpaceTableReservation> repository, IPushNotificationsService pushNotificationsService) : IRequestHandler<DeclineSpaceTableReservationCommand>
{
    readonly IRepository<SpaceTableReservation> repository = repository;
    readonly IPushNotificationsService pushNotificationsService = pushNotificationsService;

    public async Task Handle(DeclineSpaceTableReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await this.repository.GetByAsyncWithTracking(x => x.Id == request.ReservationId, cancellationToken)
            ?? throw new NotFoundException(nameof(SpaceTableReservation), request.ReservationId);

        reservation.Status = ReservationStatus.Declined;
        reservation.InternalNote = request.InternalNote;
        reservation.PublicNote = request.PublicNote;

        await this.repository.SaveChangesAsync(cancellationToken);

        await this.pushNotificationsService
            .SendNotificationToUsersAsync(
                new List<GetUserByRoleModel>
                {
                    { new() { Id = reservation.UserId } }
                },
                new SpaceTableDeclinedMessage(reservation.NumberOfGuests, reservation.ReservationDate),
                cancellationToken);
    }
}

