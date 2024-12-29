using DH.Domain.Adapters.Authentication.Models;
using DH.Domain.Adapters.PushNotifications.Messages;
using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Repositories;
using MediatR;
using DH.Domain.Adapters.Reservations;
using DH.OperationResultCore.Exceptions;

namespace DH.Application.Games.Commands.Games;

public record ApproveGameReservationCommand(int Id, string InternalNote, string PublicNote) : IRequest;

internal class ApproveGameReservationCommandHandler(IRepository<GameReservation> repository, IRepository<Game> gameRepository, ReservationCleanupQueue queue, IPushNotificationsService pushNotificationsService) : IRequestHandler<ApproveGameReservationCommand>
{
    readonly IRepository<GameReservation> repository = repository;
    readonly IRepository<Game> gameRepository = gameRepository;
    readonly ReservationCleanupQueue queue = queue;
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

        //TODO: Additional minutes can be tenantSettings
        this.queue.AddReservationCleaningJob(reservation.Id, ReservationType.Game, reservation.ReservationDate.AddMinutes(10));

        await this.pushNotificationsService
            .SendNotificationToUsersAsync(
                new List<GetUserByRoleModel>
                {
                    { new() { Id = reservation.UserId } }
                },
                new GameReservationApprovedMessage(reservation.NumberOfGuests, game!.Name, reservation.ReservationDate),
                cancellationToken);
    }
}
