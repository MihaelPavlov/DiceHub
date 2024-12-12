using DH.Domain.Adapters.Authentication.Models;
using DH.Domain.Adapters.PushNotifications.Messages;
using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Exceptions;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Games.Commands.Games;

public record DeclineGameReservationCommand(int Id, string InternalNote, string PublicNote) : IRequest;

internal class DeclineGameReservationCommandHandler(IRepository<GameReservation> repository, IRepository<Game> gameRepository, IPushNotificationsService pushNotificationsService) : IRequestHandler<DeclineGameReservationCommand>
{
    readonly IRepository<GameReservation> repository = repository;
    readonly IRepository<Game> gameRepository = gameRepository;
    readonly IPushNotificationsService pushNotificationsService = pushNotificationsService;

    public async Task Handle(DeclineGameReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await this.repository.GetByAsyncWithTracking(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(GameReservation), request.Id);

        reservation.Status = ReservationStatus.Declined;
        reservation.InternalNote = request.InternalNote;
        reservation.PublicNote = request.PublicNote;

        await this.repository.SaveChangesAsync(cancellationToken);
        var game = await this.gameRepository.GetByAsync(x => x.Id == reservation.GameId, cancellationToken);

        await this.pushNotificationsService
            .SendNotificationToUsersAsync(
                new List<GetUserByRoleModel>
                {
                    { new() { Id = reservation.UserId } }
                },
                new GameReservationDeclinedMessage(reservation.NumberOfGuests, game!.Name, reservation.ReservationDate),
                cancellationToken);
    }
}
