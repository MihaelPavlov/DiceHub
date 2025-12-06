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

namespace DH.Application.Games.Commands.Games;

public record DeclineGameReservationCommand(int Id, string InternalNote, string PublicNote) : IRequest;

internal class DeclineGameReservationCommandHandler(
    IRepository<GameReservation> repository, IRepository<Game> gameRepository,
    IReservationCleanupQueue queue, IStatisticQueuePublisher statisticQueuePublisher,
    IPushNotificationsService pushNotificationsService) : IRequestHandler<DeclineGameReservationCommand>
{
    readonly IRepository<GameReservation> repository = repository;
    readonly IRepository<Game> gameRepository = gameRepository;
    readonly IPushNotificationsService pushNotificationsService = pushNotificationsService;
    readonly IStatisticQueuePublisher statisticQueuePublisher = statisticQueuePublisher;
    readonly IReservationCleanupQueue queue = queue;

    public async Task Handle(DeclineGameReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await this.repository.GetByAsyncWithTracking(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(GameReservation), request.Id);

        reservation.Status = ReservationStatus.Declined;
        reservation.InternalNote = request.InternalNote;
        reservation.PublicNote = request.PublicNote;

        await this.repository.SaveChangesAsync(cancellationToken);
        var game = await this.gameRepository.GetByAsync(x => x.Id == reservation.GameId, cancellationToken);

        DateTime newCleanupTime = DateTime.UtcNow.AddMinutes(2);
        await this.queue.UpdateReservationCleaningJob(reservation.Id, ReservationType.Game, newCleanupTime);

        await this.statisticQueuePublisher.PublishAsync(new ReservationProcessingOutcomeJob(
            reservation.UserId, ReservationOutcome.Cancelled, ReservationType.Game, reservation.Id, DateTime.UtcNow));

        var payload = new GameReservationDeclinedNotification
        {
            ReservationDate = reservation.ReservationDate,
            GameName = game!.Name,
            NumberOfGuests = reservation.NumberOfGuests
        };

        await this.pushNotificationsService
            .SendNotificationToUsersAsync([reservation.UserId], payload, cancellationToken);
    }
}
