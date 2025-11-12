using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Adapters.PushNotifications.Messages;
using DH.Domain.Adapters.Statistics;
using DH.Domain.Adapters.Statistics.Services;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;

namespace DH.Application.SpaceManagement.Commands;

public record CancelSpaceTableReservationCommand(int ReservationId) : IRequest;

internal class CancelSpaceTableReservationCommandHandler(
    IRepository<SpaceTableReservation> repository,
    IStatisticQueuePublisher statisticQueuePublisher,
    IPushNotificationsService pushNotificationsService) : IRequestHandler<CancelSpaceTableReservationCommand>
{
    readonly IRepository<SpaceTableReservation> repository = repository;
    readonly IPushNotificationsService pushNotificationsService = pushNotificationsService;
    readonly IStatisticQueuePublisher statisticQueuePublisher = statisticQueuePublisher;

    public async Task Handle(CancelSpaceTableReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await this.repository.GetByAsyncWithTracking(x => x.Id == request.ReservationId, cancellationToken)
            ?? throw new NotFoundException(nameof(SpaceTableReservation), request.ReservationId);

        reservation.Status = ReservationStatus.Expired;
        reservation.IsReservationSuccessful = false;
        reservation.IsActive = false;

        await this.repository.SaveChangesAsync(cancellationToken);

        await this.statisticQueuePublisher.PublishAsync(new StatisticJobQueue.ReservationProcessingOutcomeJob(
            reservation.UserId, ReservationOutcome.Cancelled, ReservationType.Table, reservation.Id, DateTime.UtcNow));

        var payload = new SpaceTableCancelNotification
        {
            NumberOfGuests = reservation.NumberOfGuests,
            ReservationDate = reservation.ReservationDate
        };

        await this.pushNotificationsService
            .SendNotificationToUsersAsync([reservation.UserId], payload, cancellationToken);
    }
}
