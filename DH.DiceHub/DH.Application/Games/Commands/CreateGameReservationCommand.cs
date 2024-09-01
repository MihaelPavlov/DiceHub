using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Data;
using DH.Domain.Adapters.Scheduling;
using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Models.GameModels.Commands;
using DH.Domain.Repositories;
using DH.Domain.Services;
using MediatR;
using System.Runtime.InteropServices;

namespace DH.Application.Games.Commands;

public record CreateGameReservationCommand(CreateGameReservationModel Reservation) : IRequest;

internal class CreateGameReservationCommandHandler : IRequestHandler<CreateGameReservationCommand>
{
    readonly IGameService gameService;
    readonly IRepository<GameReservation> repository;
    readonly IJobManager jobManager;
    readonly IUserContext userContext;

    public CreateGameReservationCommandHandler(IGameService gameService, IRepository<GameReservation> repository, IJobManager jobManager, IUserContext userContext)
    {
        this.gameService = gameService;
        this.repository = repository;
        this.jobManager = jobManager;
        this.userContext = userContext;
    }

    public async Task Handle(CreateGameReservationCommand request, CancellationToken cancellationToken)
    {
        var reservationOfCurrentUser = await this.repository.GetByAsync(x => x.UserId == this.userContext.UserId && !x.IsExpired, cancellationToken);

        if (reservationOfCurrentUser != null)
            throw new ValidationErrorsException("reservationExist", "There is already one reservation");

        var reservation = new GameReservation
        {
            GameId = request.Reservation.GameId,
            UserId = this.userContext.UserId,
            ReservationDate = DateTime.Now,
            ReservedDurationMinutes = request.Reservation.DurationInMinutes
        };

        await this.gameService.CreateReservation(reservation, cancellationToken);

        await this.jobManager.CreateReservationJob(reservation.Id, reservation.ReservationDate, reservation.ReservedDurationMinutes);
    }
}

