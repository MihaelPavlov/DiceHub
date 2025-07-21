using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Authentication.Models.Enums;
using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Adapters.PushNotifications.Messages;
using DH.Domain.Adapters.Reservations;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Repositories;
using DH.Domain.Services.TenantSettingsService;
using DH.OperationResultCore.Exceptions;
using MediatR;

namespace DH.Application.SpaceManagement.Commands;

public record CreateSpaceTableReservationCommand(int NumberOfGuests, DateTime ReservationDate) : IRequest;

internal class CreateSpaceTableReservationCommandHandler(
    IRepository<SpaceTableReservation> repository,
    IUserContext userContext,
    IPushNotificationsService pushNotificationsService,
    IUserService userService,
    ITenantSettingsCacheService tenantSettingsCacheService,
    ReservationCleanupQueue queue) : IRequestHandler<CreateSpaceTableReservationCommand>
{
    readonly IRepository<SpaceTableReservation> repository = repository;
    readonly IUserContext userContext = userContext;
    readonly IPushNotificationsService pushNotificationsService = pushNotificationsService;
    readonly IUserService userService = userService;
    readonly ITenantSettingsCacheService tenantSettingsCacheService = tenantSettingsCacheService;
    readonly ReservationCleanupQueue queue = queue;

    public async Task Handle(CreateSpaceTableReservationCommand request, CancellationToken cancellationToken)
    {
        var isUserHaveActiveReservation = await this.repository.GetByAsync(x => x.IsActive && x.UserId == this.userContext.UserId, cancellationToken);

        if (isUserHaveActiveReservation != null)
            throw new BadRequestException("User already have an active reservation");

        DateTime reservationDate = TimeZoneInfo.ConvertTimeFromUtc(request.ReservationDate, TimeZoneInfo.Local).ToUniversalTime();
        var reservation = await this.repository.AddAsync(new SpaceTableReservation
        {
            UserId = this.userContext.UserId,
            CreatedDate = DateTime.UtcNow,
            ReservationDate = reservationDate,
            IsReservationSuccessful = false,
            IsActive = true,
            NumberOfGuests = request.NumberOfGuests,
            Status = ReservationStatus.Pending,
        }, cancellationToken);

        var settings = await this.tenantSettingsCacheService.GetGlobalTenantSettingsAsync(cancellationToken);

        this.queue.AddReservationCleaningJob(reservation.Id, ReservationType.Table, reservationDate.AddMinutes(settings.BonusTimeAfterReservationExpiration));

        var users = await this.userService.GetUserListByRole(Role.Staff, cancellationToken);
        var userIds = users.Select(user => user.Id).ToList();
        await this.pushNotificationsService
            .SendNotificationToUsersAsync(
                userIds,
                new SpaceTableReservationManagementReminder(request.NumberOfGuests, reservationDate),
                cancellationToken);
    }
}