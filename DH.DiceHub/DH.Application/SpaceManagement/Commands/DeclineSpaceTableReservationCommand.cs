using DH.Domain.Adapters.PushNotifications.Messages;
using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Repositories;
using MediatR;
using DH.Domain.Adapters.Reservations;
using DH.OperationResultCore.Exceptions;
using DH.Domain.Adapters.Statistics.Services;
using DH.Domain.Adapters.Statistics;

namespace DH.Application.SpaceManagement.Commands;

public record DeclineSpaceTableReservationCommand(int ReservationId, string InternalNote, string PublicNote) : IRequest;

internal class DeclineSpaceTableReservationCommandHandler(
    IRepository<SpaceTableReservation> repository,
    ReservationCleanupQueue queue,
    IStatisticQueuePublisher statisticQueuePublisher,
    IPushNotificationsService pushNotificationsService) : IRequestHandler<DeclineSpaceTableReservationCommand>
{
    readonly IRepository<SpaceTableReservation> repository = repository;
    readonly ReservationCleanupQueue queue = queue;
    readonly IPushNotificationsService pushNotificationsService = pushNotificationsService;
    readonly IStatisticQueuePublisher statisticQueuePublisher = statisticQueuePublisher;

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
        this.queue.UpdateReservationCleaningJob(reservation.Id, newCleanupTime);

        await this.statisticQueuePublisher.PublishAsync(new StatisticJobQueue.ReservationProcessingOutcomeJob(
            reservation.UserId, ReservationOutcome.Cancelled, ReservationType.Table, reservation.Id, DateTime.UtcNow));

        await this.pushNotificationsService
            .SendNotificationToUsersAsync(
                [reservation.UserId],
                new SpaceTableDeclinedMessage(reservation.NumberOfGuests, reservation.ReservationDate),
        cancellationToken);
    }
}
