using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Authentication.Models.Enums;
using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Adapters.PushNotifications.Messages;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;
using System.Globalization;

namespace DH.Application.SpaceManagement.Commands;

public record CreateSpaceTableReservationCommand(int NumberOfGuests, DateTime ReservationDate, string Time) : IRequest;

internal class CreateSpaceTableReservationCommandHandler(IRepository<SpaceTableReservation> repository, IUserContext userContext, IPushNotificationsService pushNotificationsService, IUserService userService) : IRequestHandler<CreateSpaceTableReservationCommand>
{
    readonly IRepository<SpaceTableReservation> repository = repository;
    readonly IUserContext userContext = userContext;
    readonly IPushNotificationsService pushNotificationsService = pushNotificationsService;
    readonly IUserService userService = userService;

    public async Task Handle(CreateSpaceTableReservationCommand request, CancellationToken cancellationToken)
    {
        var isUserHaveActiveReservation = await this.repository.GetByAsync(x => x.IsActive && x.UserId == this.userContext.UserId, cancellationToken);

        if (isUserHaveActiveReservation != null)
            throw new BadRequestException("User already have an active reservation");

        await this.repository.AddAsync(new SpaceTableReservation
        {
            UserId = this.userContext.UserId,
            CreatedDate = DateTime.UtcNow,
            ReservationDate = CombineDateAndTime(request.ReservationDate, request.Time).ToUniversalTime(),
            IsReservationSuccessful = false,
            IsActive = true,
            NumberOfGuests = request.NumberOfGuests,
            Status = ReservationStatus.None,
        }, cancellationToken);

        var users = await this.userService.GetUserListByRole(Role.Staff, cancellationToken);

        await this.pushNotificationsService
            .SendNotificationToUsersAsync(
                users,
                new SpaceTableReservationManagementReminder(request.NumberOfGuests, CombineDateAndTime(request.ReservationDate, request.Time)),
                cancellationToken);
    }

    public DateTime CombineDateAndTime(DateTime reservationDate, string time)
    {
        // Parse the time string into a TimeSpan
        if (!TimeSpan.TryParseExact(time, "hh\\:mm", CultureInfo.InvariantCulture, out var parsedTime))
            throw new FormatException("Time format is invalid. Expected format is 'HH:mm'.");

        // Combine the date and time
        return reservationDate.Date.Add(parsedTime);
    }
}