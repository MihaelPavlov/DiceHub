using DH.Domain.Adapters.Authentication.Models;
using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Adapters.PushNotifications.Messages;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Exceptions;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.SpaceManagement.Commands;

public record ApproveSpaceTableReservationCommand(int ReservationId) : IRequest;

internal class ApproveSpaceTableReservationCommandHandler(IRepository<SpaceTableReservation> repository, IPushNotificationsService pushNotificationsService) : IRequestHandler<ApproveSpaceTableReservationCommand>
{
    readonly IRepository<SpaceTableReservation> repository = repository;
    readonly IPushNotificationsService pushNotificationsService = pushNotificationsService;

    public async Task Handle(ApproveSpaceTableReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await this.repository.GetByAsyncWithTracking(x => x.Id == request.ReservationId, cancellationToken)
            ?? throw new NotFoundException(nameof(SpaceTableReservation), request.ReservationId);

        reservation.Status = ReservationStatus.Accepted;

        await this.repository.SaveChangesAsync(cancellationToken);

        await this.pushNotificationsService
            .SendNotificationToUsersAsync(
                new List<GetUserByRoleModel>
                {
                    { new() { Id = reservation.UserId } }
                }, 
                new SpaceTableApprovedMessage(reservation.NumberOfGuests, reservation.ReservationDate),
                cancellationToken);
    }
}
