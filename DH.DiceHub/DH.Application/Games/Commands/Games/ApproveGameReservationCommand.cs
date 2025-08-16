using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Adapters.PushNotifications.Messages;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;

namespace DH.Application.Games.Commands.Games;

public record ApproveGameReservationCommand(int Id, string InternalNote, string PublicNote) : IRequest;

internal class ApproveGameReservationCommandHandler(
    IRepository<GameReservation> repository, IRepository<Game> gameRepository,
    IPushNotificationsService pushNotificationsService) : IRequestHandler<ApproveGameReservationCommand>
{
    readonly IRepository<GameReservation> repository = repository;
    readonly IRepository<Game> gameRepository = gameRepository;
    readonly IPushNotificationsService pushNotificationsService = pushNotificationsService;

    public async Task Handle(ApproveGameReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await this.repository.GetByAsyncWithTracking(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(GameReservation), request.Id);

        reservation.Status = ReservationStatus.Accepted;
        reservation.InternalNote = request.InternalNote;
        reservation.PublicNote = request.PublicNote;

        await this.repository.SaveChangesAsync(cancellationToken);

        var game = await this.gameRepository.GetByAsync(x => x.Id == reservation.GameId, cancellationToken);

        var payload = new GameReservationApprovedNotification
        {
            ReservationDate = reservation.ReservationDate,
            GameName = game!.Name,
            NumberOfGuests = reservation.NumberOfGuests
        };

        await this.pushNotificationsService
            .SendNotificationToUsersAsync(
                [reservation.UserId], payload, cancellationToken);
    }
}
