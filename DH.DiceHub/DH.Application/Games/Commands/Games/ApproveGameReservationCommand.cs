using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Adapters.PushNotifications.Messages;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Helpers;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DH.Application.Games.Commands.Games;

public record ApproveGameReservationCommand(int Id, string InternalNote, string PublicNote) : IRequest;

internal class ApproveGameReservationCommandHandler(
    IRepository<GameReservation> repository, IRepository<Game> gameRepository,
    IPushNotificationsService pushNotificationsService, IUserContext userContext,
    ILogger<ApproveGameReservationCommandHandler> logger) : IRequestHandler<ApproveGameReservationCommand>
{
    readonly IRepository<GameReservation> repository = repository;
    readonly IRepository<Game> gameRepository = gameRepository;
    readonly IPushNotificationsService pushNotificationsService = pushNotificationsService;
    readonly IUserContext userContext = userContext;
    readonly ILogger<ApproveGameReservationCommandHandler> logger = logger;

    public async Task Handle(ApproveGameReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await this.repository.GetByAsyncWithTracking(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(GameReservation), request.Id);

        reservation.Status = ReservationStatus.Accepted;
        reservation.InternalNote = request.InternalNote;
        reservation.PublicNote = request.PublicNote;

        await this.repository.SaveChangesAsync(cancellationToken);

        var game = await this.gameRepository.GetByAsync(x => x.Id == reservation.GameId, cancellationToken);

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
                new GameReservationApprovedMessage(reservation.NumberOfGuests, game!.Name, userLocalReservationDate, isUtcFallback),
                cancellationToken);
    }
}
