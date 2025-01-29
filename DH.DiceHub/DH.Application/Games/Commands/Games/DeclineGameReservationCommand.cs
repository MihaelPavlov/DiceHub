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

namespace DH.Application.Games.Commands.Games;

public record DeclineGameReservationCommand(int Id, string InternalNote, string PublicNote) : IRequest;

internal class DeclineGameReservationCommandHandler(IRepository<GameReservation> repository, IRepository<Game> gameRepository, IEventPublisherService eventPublisherService, IPushNotificationsService pushNotificationsService) : IRequestHandler<DeclineGameReservationCommand>
{
    readonly IRepository<GameReservation> repository = repository;
    readonly IRepository<Game> gameRepository = gameRepository;
    readonly IPushNotificationsService pushNotificationsService = pushNotificationsService;
    readonly IEventPublisherService eventPublisherService = eventPublisherService;

    public async Task Handle(DeclineGameReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await this.repository.GetByAsyncWithTracking(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(GameReservation), request.Id);

        reservation.Status = ReservationStatus.Declined;
        reservation.InternalNote = request.InternalNote;
        reservation.PublicNote = request.PublicNote;

        await this.repository.SaveChangesAsync(cancellationToken);
        var game = await this.gameRepository.GetByAsync(x => x.Id == reservation.GameId, cancellationToken);

        //Todo: maybe if we are declining the reservation we can adjust the cleaning time, to be 10mins after the declining of the reservation.
        // For example if the resevation is for after 50minutes. and the staff declined it at the 10min, it's not nesscarry to wait additional 50mins for the reservation to be cleaned.

        await eventPublisherService.PublishReservationProcessingOutcomeMessage(ReservationOutcome.Cancelled.ToString(), reservation.UserId, ReservationType.Game.ToString(), reservation.Id);

        await this.pushNotificationsService
            .SendNotificationToUsersAsync(
                new List<GetUserByRoleModel>
                {
                    { new() { Id = reservation.UserId } }
                },
                new GameReservationDeclinedMessage(reservation.NumberOfGuests, game!.Name, reservation.ReservationDate.ToLocalTime()),
                cancellationToken);
    }
}
