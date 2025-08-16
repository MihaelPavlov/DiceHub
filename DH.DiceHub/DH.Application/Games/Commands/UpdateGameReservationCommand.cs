using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Adapters.PushNotifications.Messages;
using DH.Domain.Entities;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;

namespace DH.Application.Games.Commands;

public record UpdateGameReservationCommand(int Id, string PublicNote, string InternalNote) : IRequest;

internal class UpdateGameReservationCommandHandler(
    IRepository<GameReservation> repository,
    IPushNotificationsService pushNotificationsService) : IRequestHandler<UpdateGameReservationCommand>
{
    readonly IRepository<GameReservation> repository = repository;
    readonly IPushNotificationsService pushNotificationsService = pushNotificationsService;

    public async Task Handle(UpdateGameReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await this.repository.GetByAsyncWithTracking(x => x.Id == request.Id, cancellationToken)
             ?? throw new NotFoundException(nameof(GameReservation), request.Id);

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
            var payload = new GameReservationPublicNoteUpdatedNotification
            {
                ReservationDate = reservation.ReservationDate,
                NumberOfGuests = reservation.NumberOfGuests
            };

            await this.pushNotificationsService
                .SendNotificationToUsersAsync([reservation.UserId], payload, cancellationToken);
        }
    }
}
