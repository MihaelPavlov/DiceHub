using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Adapters.PushNotifications.Messages;
using DH.Domain.Adapters.Statistics;
using DH.Domain.Adapters.Statistics.Services;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;

namespace DH.Application.Games.Commands.Games;

public record CancelGameReservationCommand(int Id) : IRequest;

internal class CancelGameReservationCommandHandler(
    IRepository<GameReservation> repository, IRepository<Game> gameRepository,
    IRepository<GameInventory> gameInventoryRepository, IStatisticQueuePublisher statisticQueuePublisher,
    IPushNotificationsService pushNotificationsService) : IRequestHandler<CancelGameReservationCommand>
{
    readonly IRepository<GameReservation> repository = repository;
    readonly IRepository<GameInventory> gameInventoryRepository = gameInventoryRepository;
    readonly IRepository<Game> gameRepository = gameRepository;
    readonly IPushNotificationsService pushNotificationsService = pushNotificationsService;
    readonly IStatisticQueuePublisher statisticQueuePublisher = statisticQueuePublisher;

    public async Task Handle(CancelGameReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await this.repository.GetByAsyncWithTracking(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(GameReservation), request.Id);

        reservation.Status = ReservationStatus.Expired;
        reservation.IsReservationSuccessful = false;
        reservation.IsActive = false;

        var inventory = await gameInventoryRepository.GetByAsyncWithTracking(x => x.GameId == reservation.GameId, cancellationToken);

        if (inventory != null && inventory.AvailableCopies < inventory.TotalCopies)
            inventory.AvailableCopies++;

        await this.repository.SaveChangesAsync(cancellationToken);

        var game = await this.gameRepository.GetByAsync(x => x.Id == reservation.GameId, cancellationToken);

        await this.statisticQueuePublisher.PublishAsync(new StatisticJobQueue.ReservationProcessingOutcomeJob(
            reservation.UserId, ReservationOutcome.Cancelled, ReservationType.Game, reservation.Id, DateTime.UtcNow));

        var payload = new GameReservationCancelledNotification
        {
            ReservationDate = reservation.ReservationDate,
            GameName = game!.Name,
            NumberOfGuests = reservation.NumberOfGuests
        };

        await this.pushNotificationsService
            .SendNotificationToUsersAsync([reservation.UserId], payload, cancellationToken);
    }
}
