using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Adapters.PushNotifications.Messages;
using DH.Domain.Adapters.Reservations;
using DH.Domain.Adapters.Statistics;
using DH.Domain.Adapters.Statistics.Services;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;

namespace DH.Application.SpaceManagement.Commands;

public record DeclineSpaceTableReservationCommand(int ReservationId, string InternalNote, string PublicNote) : IRequest;

internal class DeclineSpaceTableReservationCommandHandler(
    IRepository<SpaceTableReservation> repository,
    IReservationCleanupQueue queue,
    IStatisticQueuePublisher statisticQueuePublisher,
    IPushNotificationsService pushNotificationsService) : IRequestHandler<DeclineSpaceTableReservationCommand>
{
    readonly IRepository<SpaceTableReservation> repository = repository;
    readonly IReservationCleanupQueue queue = queue;
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

        DateTime newCleanupTime = DateTime.UtcNow.AddMinutes(2);
        await this.queue.UpdateReservationCleaningJob(reservation.Id, ReservationType.Table, newCleanupTime);

        await this.statisticQueuePublisher.PublishAsync(new ReservationProcessingOutcomeJob(
            reservation.UserId, ReservationOutcome.Cancelled, ReservationType.Table, reservation.Id, DateTime.UtcNow));

        var payload = new SpaceTableDeclinedNotification
        {
            NumberOfGuests = reservation.NumberOfGuests,
            ReservationDate = reservation.ReservationDate
        };

        await this.pushNotificationsService
            .SendNotificationToUsersAsync([reservation.UserId], payload, cancellationToken);
    }
}
