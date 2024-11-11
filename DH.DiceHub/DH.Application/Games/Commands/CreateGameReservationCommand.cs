using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Authentication.Models.Enums;
using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Adapters.PushNotifications.Messages;
using DH.Domain.Adapters.Scheduling;
using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Models.GameModels.Commands;
using DH.Domain.Repositories;
using DH.Domain.Services;
using MediatR;

namespace DH.Application.Games.Commands;

public record CreateGameReservationCommand(CreateGameReservationModel Reservation) : IRequest;

internal class CreateGameReservationCommandHandler : IRequestHandler<CreateGameReservationCommand>
{
    readonly IGameService gameService;
    readonly IRepository<GameReservation> repository;
    readonly IRepository<Game> gameRepository;
    readonly IJobManager jobManager;
    readonly IUserContext userContext;
    readonly IPushNotificationsService pushNotificationsService;
    readonly IUserService userService;

    public CreateGameReservationCommandHandler(IGameService gameService, IRepository<GameReservation> repository, IJobManager jobManager, IUserContext userContext, IPushNotificationsService pushNotificationsService, IUserService userService, IRepository<Game> gameRepository)
    {
        this.gameService = gameService;
        this.repository = repository;
        this.jobManager = jobManager;
        this.userContext = userContext;
        this.pushNotificationsService = pushNotificationsService;
        this.userService = userService;
        this.gameRepository = gameRepository;
    }

    public async Task Handle(CreateGameReservationCommand request, CancellationToken cancellationToken)
    {
        var activeReservationOfCurrentUser = await this.repository.GetByAsync(x => x.UserId == this.userContext.UserId && x.IsActive, cancellationToken);

        if (activeReservationOfCurrentUser != null)
            throw new ValidationErrorsException("reservationExist", "There is already one reservation");

        var reservation = new GameReservation
        {
            GameId = request.Reservation.GameId,
            UserId = this.userContext.UserId,
            ReservationDate = DateTime.Now,
            ReservedDurationMinutes = request.Reservation.DurationInMinutes,
            IsActive = true,
            PeopleCount = request.Reservation.PeopleCount,
        };

        await this.gameService.CreateReservation(reservation, cancellationToken);

        await this.jobManager.CreateReservationJob(reservation.Id, reservation.ReservationDate, reservation.ReservedDurationMinutes);

        //TODO: Change to Role.Staff
        var users = await this.userService.GetUserListByRole(Role.SuperAdmin, cancellationToken);
        var getUsers = await this.userService.GetUserListByIds([this.userContext.UserId], cancellationToken);
        var currentUser = getUsers.First();

        var game = await this.gameRepository.GetByAsync(x => x.Id == request.Reservation.GameId, cancellationToken);

        DateTime reservationEndTime = reservation.ReservationDate.AddMinutes(reservation.ReservedDurationMinutes);

        await this.pushNotificationsService.SendNotificationToUsersAsync(users, new GameReservationManagementReminder(game!.Name,currentUser.UserName,request.Reservation.PeopleCount, reservationEndTime), cancellationToken);
    }
}

