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

namespace DH.Application.Games.Commands.Games;

public record DeclineGameReservationCommand(int Id, string InternalNote, string PublicNote) : IRequest;

internal class DeclineGameReservationCommandHandler(
    IRepository<GameReservation> repository, IRepository<Game> gameRepository,
    ReservationCleanupQueue queue, IStatisticQueuePublisher statisticQueuePublisher,
    IPushNotificationsService pushNotificationsService, IUserContext userContext,
    ILogger<DeclineGameReservationCommandHandler> logger) : IRequestHandler<DeclineGameReservationCommand>
{
    readonly IRepository<GameReservation> repository = repository;
    readonly IRepository<Game> gameRepository = gameRepository;
    readonly IPushNotificationsService pushNotificationsService = pushNotificationsService;
    readonly IStatisticQueuePublisher statisticQueuePublisher = statisticQueuePublisher;
    readonly ReservationCleanupQueue queue = queue;
    readonly IUserContext userContext = userContext;
    readonly ILogger<DeclineGameReservationCommandHandler> logger = logger;

    public async Task Handle(DeclineGameReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await this.repository.GetByAsyncWithTracking(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(GameReservation), request.Id);

        reservation.Status = ReservationStatus.Declined;
        reservation.InternalNote = request.InternalNote;
        reservation.PublicNote = request.PublicNote;

        await this.repository.SaveChangesAsync(cancellationToken);
        var game = await this.gameRepository.GetByAsync(x => x.Id == reservation.GameId, cancellationToken);

        //TODO: This additional minutes can be tenantSettings
        DateTime newCleanupTime = DateTime.UtcNow.AddMinutes(2);
        this.queue.UpdateReservationCleaningJob(reservation.Id, newCleanupTime);

        await this.statisticQueuePublisher.PublishAsync(new StatisticJobQueue.ReservationProcessingOutcomeJob(
            reservation.UserId, ReservationOutcome.Cancelled, ReservationType.Game, reservation.Id, DateTime.UtcNow));

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
                new GameReservationDeclinedMessage(reservation.NumberOfGuests, game!.Name, userLocalReservationDate, isUtcFallback),
                cancellationToken);
    }
}
