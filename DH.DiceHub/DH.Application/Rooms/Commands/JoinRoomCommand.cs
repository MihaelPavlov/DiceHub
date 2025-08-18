using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.Localization;
using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Adapters.PushNotifications.Messages;
using DH.Domain.Entities;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;

namespace DH.Application.Rooms.Commands;

public record JoinRoomCommand(int Id) : IRequest;

internal class JoinRoomCommandHandler : IRequestHandler<JoinRoomCommand>
{
    readonly IRepository<Room> roomsRepository;
    readonly IRepository<RoomParticipant> roomParticipantsRepository;
    readonly IUserContext userContext;
    readonly IUserService userService;
    readonly IPushNotificationsService pushNotificationsService;
    readonly ILocalizationService localizer;

    public JoinRoomCommandHandler(
        IRepository<Room> roomsRepository,
        IRepository<RoomParticipant> roomParticipantsRepository,
        IUserContext userContext, IUserService userService,
        IPushNotificationsService pushNotificationsService,
        ILocalizationService localizer)
    {
        this.roomsRepository = roomsRepository;
        this.roomParticipantsRepository = roomParticipantsRepository;
        this.userContext = userContext;
        this.userService = userService;
        this.pushNotificationsService = pushNotificationsService;
        this.localizer = localizer;
    }

    public async Task Handle(JoinRoomCommand request, CancellationToken cancellationToken)
    {
        var room = await this.roomsRepository.GetByAsync(g => g.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Room), request.Id);

        var activeRoomParticipantList = await this.roomParticipantsRepository
            .GetWithPropertiesAsync(
                x => x.RoomId == room.Id && !x.IsDeleted,
                x => new RoomParticipant
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    IsDeleted = x.IsDeleted,
                    JoinedAt = x.JoinedAt,
                    RoomId = x.RoomId,
                },
                cancellationToken);

        if (room.MaxParticipants == activeRoomParticipantList.Count)
            throw new ValidationErrorsException("MaxPeople", this.localizer["RoomMaxPeople"]);

        var roomParticipantExist = await this.roomParticipantsRepository
            .GetByAsync(
                x => x.RoomId == room.Id && x.UserId == this.userContext.UserId,
                cancellationToken);

        if (roomParticipantExist != null)
        {
            roomParticipantExist.IsDeleted = false;
            await this.roomParticipantsRepository.Update(roomParticipantExist, cancellationToken);
        }
        else
        {
            var roomParticipant = new RoomParticipant { UserId = this.userContext.UserId, Room = room, JoinedAt = DateTime.UtcNow };
            await this.roomParticipantsRepository.AddAsync(roomParticipant, cancellationToken);
        }
        var user = await this.userService.GetUserById(this.userContext.UserId, cancellationToken);
        var payload = new RoomParticipantJoinedNotification
        {
            ParticipantName = user?.UserName ?? this.localizer["NotProvided"],
            RoomName = room.Name
        };
        await this.pushNotificationsService.SendNotificationToUsersAsync([room.UserId], payload, cancellationToken);
    }
}