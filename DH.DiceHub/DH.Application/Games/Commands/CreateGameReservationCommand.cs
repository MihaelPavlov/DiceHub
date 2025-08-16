using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Authentication.Models.Enums;
using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.Localization;
using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Adapters.PushNotifications.Messages;
using DH.Domain.Adapters.Reservations;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Helpers;
using DH.Domain.Models.GameModels.Commands;
using DH.Domain.Repositories;
using DH.Domain.Services;
using DH.Domain.Services.TenantSettingsService;
using DH.OperationResultCore.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DH.Application.Games.Commands;

public record CreateGameReservationCommand(CreateGameReservationModel Reservation) : IRequest;

internal class CreateGameReservationCommandHandler(
    IGameService gameService,
    IRepository<GameReservation> repository,
    IUserContext userContext, ReservationCleanupQueue queue,
    IPushNotificationsService pushNotificationsService,
    IUserService userService,
    IRepository<Game> gameRepository,
    ITenantSettingsCacheService tenantSettingsCacheService,
    ILogger<CreateGameReservationCommandHandler> logger,
    ILocalizationService localizer) : IRequestHandler<CreateGameReservationCommand>
{
    readonly IGameService gameService = gameService;
    readonly IRepository<GameReservation> repository = repository;
    readonly IRepository<Game> gameRepository = gameRepository;
    readonly IUserContext userContext = userContext;
    readonly IPushNotificationsService pushNotificationsService = pushNotificationsService;
    readonly IUserService userService = userService;
    readonly ITenantSettingsCacheService tenantSettingsCacheService = tenantSettingsCacheService;
    readonly ReservationCleanupQueue queue = queue;
    readonly ILogger<CreateGameReservationCommandHandler> logger = logger;
    readonly ILocalizationService localizer = localizer;

    public async Task Handle(CreateGameReservationCommand request, CancellationToken cancellationToken)
    {
        var activeReservationOfCurrentUser = await this.repository.GetByAsync(x => x.UserId == this.userContext.UserId && x.IsActive, cancellationToken);

        if (activeReservationOfCurrentUser != null)
            throw new ValidationErrorsException("reservationExist", this.localizer["ReservationAlreadyExists"]);

        var reservation = new GameReservation
        {
            GameId = request.Reservation.GameId,
            UserId = this.userContext.UserId,
            ReservationDate = DateTime.UtcNow.AddMinutes(request.Reservation.DurationInMinutes),
            CreatedDate = DateTime.UtcNow,
            ReservedDurationMinutes = request.Reservation.DurationInMinutes,
            IsActive = true,
            NumberOfGuests = request.Reservation.PeopleCount,
            Status = ReservationStatus.Pending
        };

        await this.gameService.CreateReservation(reservation, cancellationToken);

        var settings = await this.tenantSettingsCacheService.GetGlobalTenantSettingsAsync(cancellationToken);

        this.queue.AddReservationCleaningJob(reservation.Id, ReservationType.Game, reservation.ReservationDate.AddMinutes(settings.BonusTimeAfterReservationExpiration));

        var users = await this.userService.GetUserListByRoles([Role.Staff,Role.SuperAdmin], cancellationToken);
        var getUsers = await this.userService.GetUserListByIds([this.userContext.UserId], cancellationToken);
        var currentUser = getUsers.First();

        var game = await this.gameRepository.GetByAsync(x => x.Id == request.Reservation.GameId, cancellationToken);

        var userIds = users.Select(users => users.Id).ToList();

        var payload = new GameReservationReminderNotification
        {
            GameName = game!.Name,
            Email = currentUser.UserName,
            CountPeople = request.Reservation.PeopleCount,
            ReservationTime = reservation.ReservationDate,
        };

        await this.pushNotificationsService.SendNotificationToUsersAsync(userIds, payload, cancellationToken);
    }
}

