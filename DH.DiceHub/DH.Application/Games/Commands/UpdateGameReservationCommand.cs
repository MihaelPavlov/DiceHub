using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Adapters.PushNotifications.Messages;
using DH.Domain.Entities;
using DH.Domain.Helpers;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DH.Application.Games.Commands;

public record UpdateGameReservationCommand(int Id, string PublicNote, string InternalNote) : IRequest;

internal class UpdateGameReservationCommandHandler(
    IRepository<GameReservation> repository, IPushNotificationsService pushNotificationsService,
    IUserContext userContext, ILogger<UpdateGameReservationCommandHandler> logger) : IRequestHandler<UpdateGameReservationCommand>
{
    readonly IRepository<GameReservation> repository = repository;
    readonly IPushNotificationsService pushNotificationsService = pushNotificationsService;
    readonly IUserContext userContext = userContext;
    readonly ILogger<UpdateGameReservationCommandHandler> logger = logger;

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
            var (userLocalReservationDate, isUtcFallback) =
               TimeZoneHelper.GetUserLocalOrUtcTime(reservation.ReservationDate, this.userContext.TimeZone);

            if (isUtcFallback)
            {
                this.logger.LogWarning(
                    "User local game reservation date could not be calculated for reservation ID: {EventId}, time zone: {TimeZone}. Falling back to UTC.",
                    reservation.Id,
                    this.userContext.TimeZone);
            }

            await this.pushNotificationsService
                .SendNotificationToUsersAsync(
                    [reservation.UserId],
                    new GameReservationPublicNoteUpdatedMessage(reservation.NumberOfGuests, userLocalReservationDate, isUtcFallback),
                    cancellationToken);
        }
    }
}
