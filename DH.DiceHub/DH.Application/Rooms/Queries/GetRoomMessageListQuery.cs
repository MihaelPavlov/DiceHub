using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Models.RoomModels.Queries;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Rooms.Queries;

public record GetRoomMessageListQuery(int Id) : IRequest<List<GetRoomMessageListQueryModel>>;

internal class GetRoomMessageListQueryHandler : IRequestHandler<GetRoomMessageListQuery, List<GetRoomMessageListQueryModel>>
{
    readonly IRepository<RoomMessages> roomMessagesRepository;
    readonly IRepository<RoomParticipant> roomParticipantsRepository;
    readonly IRepository<Room> roomsRepository;
    readonly IUserService userService;
    readonly IUserContext userContext;

    public GetRoomMessageListQueryHandler(IRepository<RoomMessages> roomMessagesRepository, IRepository<Room> roomsRepository, IUserService userService, IRepository<RoomParticipant> roomParticipantsRepository, IUserContext userContext)
    {
        this.roomMessagesRepository = roomMessagesRepository;
        this.roomsRepository = roomsRepository;
        this.userService = userService;
        this.roomParticipantsRepository = roomParticipantsRepository;
        this.userContext = userContext;
    }

    public async Task<List<GetRoomMessageListQueryModel>> Handle(GetRoomMessageListQuery request, CancellationToken cancellationToken)
    {
        var room = await this.roomsRepository.GetByAsync(g => g.Id == request.Id, CancellationToken.None)
            ?? throw new NotFoundException(nameof(Room), request.Id);

        var roomParticipant = await this.roomParticipantsRepository.GetByAsync(
                g => g.UserId == this.userContext.UserId &&
                g.RoomId == room.Id &&
                !g.IsDeleted,
                CancellationToken.None);

        if (roomParticipant == null && this.userContext.UserId != room.UserId)
            throw new ForbiddenAccessException("Current user doesn't participate in the room");

        var messages = await this.roomMessagesRepository
            .GetWithPropertiesAsync(x => x.RoomId == room.Id,
            m => new GetRoomMessageListQueryModel
            {
                Id = m.Id,
                Message = m.MessageContent,
                CreatedDate = m.Timestamp,
                SenderId = m.Sender,
            }, CancellationToken.None);

        var userIds = messages.Select(x => x.SenderId).Distinct().ToArray();

        var users = await this.userService.GetUserListByIds(userIds, cancellationToken);

        foreach (var message in messages)
        {
            var user = users.FirstOrDefault(x => x.Id == message.SenderId);
            if (user != null)
                message.SenderUsername = user.UserName;
        }

        return messages;
    }
}
