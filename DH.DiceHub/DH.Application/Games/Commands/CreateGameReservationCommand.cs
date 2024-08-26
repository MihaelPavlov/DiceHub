using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Data;
using DH.Domain.Adapters.Scheduling;
using DH.Domain.Entities;
using DH.Domain.Models.GameModels.Commands;
using DH.Domain.Repositories;
using DH.Domain.Services;
using MediatR;

namespace DH.Application.Games.Commands;

public record CreateGameReservationCommand(CreateGameReservationModel Reservation) : IRequest;

internal class CreateGameReservationCommandHandler : IRequestHandler<CreateGameReservationCommand>
{
    readonly IGameService gameService;
    readonly IJobManager jobManager;
    readonly IUserContext userContext;

    public CreateGameReservationCommandHandler(IGameService gameService, IJobManager jobManager, IUserContext userContext)
    {
        this.gameService = gameService;
        this.jobManager = jobManager;
        this.userContext = userContext;
    }

    public async Task Handle(CreateGameReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = new GameReservation
        {
            GameId = request.Reservation.GameId,
            UserId = this.userContext.UserId,
            ReservationDate = DateTime.UtcNow,
            ReservedDurationMinutes = request.Reservation.DurationInMinutes
        };

        await this.gameService.CreateReservation(reservation, cancellationToken);

        await this.jobManager.CreateReservationJob(reservation.Id, reservation.ReservationDate, reservation.ReservedDurationMinutes);
    }
}

