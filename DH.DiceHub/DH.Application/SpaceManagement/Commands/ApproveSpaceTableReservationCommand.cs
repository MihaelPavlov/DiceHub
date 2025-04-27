using DH.Domain.Adapters.Authentication.Models;
using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Adapters.PushNotifications.Messages;
using DH.Domain.Adapters.Reservations;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;

namespace DH.Application.SpaceManagement.Commands;

public record ApproveSpaceTableReservationCommand(int ReservationId, string InternalNote, string PublicNote) : IRequest;

internal class ApproveSpaceTableReservationCommandHandler(IRepository<SpaceTableReservation> repository, ReservationCleanupQueue queue, IPushNotificationsService pushNotificationsService) : IRequestHandler<ApproveSpaceTableReservationCommand>
{
    readonly IRepository<SpaceTableReservation> repository = repository;
    readonly ReservationCleanupQueue queue = queue;
    readonly IPushNotificationsService pushNotificationsService = pushNotificationsService;

    public async Task Handle(ApproveSpaceTableReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await this.repository.GetByAsyncWithTracking(x => x.Id == request.ReservationId, cancellationToken)
            ?? throw new NotFoundException(nameof(SpaceTableReservation), request.ReservationId);

        reservation.Status = ReservationStatus.Accepted;
        reservation.InternalNote = request.InternalNote;
        reservation.PublicNote = request.PublicNote;

        await this.repository.SaveChangesAsync(cancellationToken);

        //TODO: Additional minutes can be tenantSettings
        this.queue.AddReservationCleaningJob(reservation.Id, ReservationType.Table, reservation.ReservationDate.AddMinutes(2));

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
