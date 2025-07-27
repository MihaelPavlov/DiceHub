using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Adapters.PushNotifications.Messages;
using DH.Domain.Adapters.Reservations;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Helpers;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DH.Application.SpaceManagement.Commands;

public record ApproveSpaceTableReservationCommand(int ReservationId, string InternalNote, string PublicNote) : IRequest;

internal class ApproveSpaceTableReservationCommandHandler(
    IRepository<SpaceTableReservation> repository, ReservationCleanupQueue queue
    , IPushNotificationsService pushNotificationsService, IUserContext userContext,
    ILogger<ApproveSpaceTableReservationCommandHandler> logger) : IRequestHandler<ApproveSpaceTableReservationCommand>
{
    readonly IRepository<SpaceTableReservation> repository = repository;
    readonly ReservationCleanupQueue queue = queue;
    readonly IPushNotificationsService pushNotificationsService = pushNotificationsService;
    readonly IUserContext userContext = userContext;
    readonly ILogger<ApproveSpaceTableReservationCommandHandler> logger = logger;

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

        var (userLocalReservationDate, isUtcFallback) =
                TimeZoneHelper.GetUserLocalOrUtcTime(reservation.ReservationDate, this.userContext.TimeZone);

        if (isUtcFallback)
        {
            this.logger.LogWarning(
                "User local table reservation date could not be calculated for reservation ID: {ReservationId}, time zone: {TimeZone}. Falling back to UTC.",
                reservation.Id,
                this.userContext.TimeZone);
        }

        await this.pushNotificationsService
            .SendNotificationToUsersAsync(
                [reservation.UserId],
                new SpaceTableApprovedMessage(reservation.NumberOfGuests, userLocalReservationDate, isUtcFallback),
                cancellationToken);
    }
}
