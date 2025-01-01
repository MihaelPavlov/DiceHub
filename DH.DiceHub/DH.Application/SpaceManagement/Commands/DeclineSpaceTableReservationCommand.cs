using DH.Domain.Adapters.Authentication.Models;
using DH.Domain.Adapters.PushNotifications.Messages;
using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Repositories;
using MediatR;
using DH.Domain.Adapters.Reservations;
using DH.OperationResultCore.Exceptions;
using DH.Domain.Services.Publisher;

namespace DH.Application.SpaceManagement.Commands;

public record DeclineSpaceTableReservationCommand(int ReservationId, string InternalNote, string PublicNote) : IRequest;

internal class DeclineSpaceTableReservationCommandHandler(IRepository<SpaceTableReservation> repository, ReservationCleanupQueue queue, IEventPublisherService eventPublisherService, IPushNotificationsService pushNotificationsService) : IRequestHandler<DeclineSpaceTableReservationCommand>
{
    readonly IRepository<SpaceTableReservation> repository = repository;
    readonly ReservationCleanupQueue queue = queue;
    readonly IPushNotificationsService pushNotificationsService = pushNotificationsService;
    readonly IEventPublisherService eventPublisherService = eventPublisherService;

    public async Task Handle(DeclineSpaceTableReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await this.repository.GetByAsyncWithTracking(x => x.Id == request.ReservationId, cancellationToken)
            ?? throw new NotFoundException(nameof(SpaceTableReservation), request.ReservationId);

        reservation.Status = ReservationStatus.Declined;
        reservation.InternalNote = request.InternalNote;
        reservation.PublicNote = request.PublicNote;

        await this.repository.SaveChangesAsync(cancellationToken);

        //TODO: Additional minutes can be tenantSettings
        this.queue.AddReservationCleaningJob(reservation.Id, ReservationType.Table, DateTime.UtcNow.AddMinutes(10));

        await eventPublisherService.PublishReservationProcessingOutcomeMessage(ReservationOutcome.Cancelled.ToString(), reservation.UserId, ReservationType.Table.ToString(), reservation.Id);

        await this.pushNotificationsService
            .SendNotificationToUsersAsync(
                new List<GetUserByRoleModel>
                {
                    { new() { Id = reservation.UserId } }
                },
                new SpaceTableDeclinedMessage(reservation.NumberOfGuests, reservation.ReservationDate),
        cancellationToken);
    }
}
