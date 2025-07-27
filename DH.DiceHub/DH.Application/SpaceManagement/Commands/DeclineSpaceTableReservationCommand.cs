using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Adapters.PushNotifications.Messages;
using DH.Domain.Adapters.Reservations;
using DH.Domain.Adapters.Statistics;
using DH.Domain.Adapters.Statistics.Services;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Helpers;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DH.Application.SpaceManagement.Commands;

public record DeclineSpaceTableReservationCommand(int ReservationId, string InternalNote, string PublicNote) : IRequest;

internal class DeclineSpaceTableReservationCommandHandler(
    IRepository<SpaceTableReservation> repository,
    ReservationCleanupQueue queue,
    IStatisticQueuePublisher statisticQueuePublisher,
    IPushNotificationsService pushNotificationsService, IUserContext userContext,
    ILogger<ApproveSpaceTableReservationCommandHandler> logger) : IRequestHandler<DeclineSpaceTableReservationCommand>
{
    readonly IRepository<SpaceTableReservation> repository = repository;
    readonly ReservationCleanupQueue queue = queue;
    readonly IPushNotificationsService pushNotificationsService = pushNotificationsService;
    readonly IStatisticQueuePublisher statisticQueuePublisher = statisticQueuePublisher;
    readonly IUserContext userContext = userContext;
    readonly ILogger<ApproveSpaceTableReservationCommandHandler> logger = logger;

    public async Task Handle(DeclineSpaceTableReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await this.repository.GetByAsyncWithTracking(x => x.Id == request.ReservationId, cancellationToken)
            ?? throw new NotFoundException(nameof(SpaceTableReservation), request.ReservationId);

        reservation.Status = ReservationStatus.Declined;
        reservation.InternalNote = request.InternalNote;
        reservation.PublicNote = request.PublicNote;

        await this.repository.SaveChangesAsync(cancellationToken);

        //TODO: Additional minutes can be tenantSettings
        DateTime newCleanupTime = DateTime.UtcNow.AddMinutes(2);
        this.queue.AddReservationCleaningJob(reservation.Id, ReservationType.Table, newCleanupTime);

        await this.statisticQueuePublisher.PublishAsync(new StatisticJobQueue.ReservationProcessingOutcomeJob(
            reservation.UserId, ReservationOutcome.Cancelled, ReservationType.Table, reservation.Id, DateTime.UtcNow));

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
                new SpaceTableDeclinedMessage(reservation.NumberOfGuests, userLocalReservationDate, isUtcFallback),
        cancellationToken);
    }
}
