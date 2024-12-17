using DH.Domain.Adapters.Authentication.Models;
using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Adapters.PushNotifications.Messages;
using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.SpaceManagement.Commands;

public record UpdateSpaceTableReservationCommand(int Id, string PublicNote, string InternalNote) : IRequest;

internal class UpdateSpaceTableReservationCommandHandler(IRepository<SpaceTableReservation> repository, IPushNotificationsService pushNotificationsService) : IRequestHandler<UpdateSpaceTableReservationCommand>
{
    readonly IRepository<SpaceTableReservation> repository = repository;
    readonly IPushNotificationsService pushNotificationsService = pushNotificationsService;

    public async Task Handle(UpdateSpaceTableReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await this.repository.GetByAsyncWithTracking(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(SpaceTableReservation), request.Id);

        if (reservation.InternalNote != request.InternalNote)
        {
            reservation.InternalNote = request.InternalNote;
        }

        var isPublicNoteUpdated = false;
        if (reservation.PublicNote != request.PublicNote)
        {
            reservation.PublicNote = request.PublicNote;
            isPublicNoteUpdated = true;
        }

        await this.repository.SaveChangesAsync(cancellationToken);

        if (isPublicNoteUpdated && reservation.IsActive)
        {
            await this.pushNotificationsService
                .SendNotificationToUsersAsync(
                    new List<GetUserByRoleModel>
                    {
                    { new() { Id = reservation.UserId } }
                    },
                    new SpaceTablePublicNoteUpdatedMessage(reservation.NumberOfGuests, reservation.ReservationDate),
                    cancellationToken);
        }
    }
}