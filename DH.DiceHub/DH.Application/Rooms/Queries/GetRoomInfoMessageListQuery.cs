using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Models.RoomModels.Queries;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Rooms.Queries;

public record GetRoomInfoMessageListQuery(int Id) : IRequest<List<GetRoomInfoMessageListQueryModel>>;

internal class GetRoomInfoMessageListQueryHandler : IRequestHandler<GetRoomInfoMessageListQuery, List<GetRoomInfoMessageListQueryModel>>
{
    readonly IRepository<RoomInfoMessage> roomInfoMessagesRepository;
    readonly IRepository<Room> roomsRepository;
    readonly IUserService userService;
    readonly IUserContext userContext;

    public GetRoomInfoMessageListQueryHandler(IRepository<RoomInfoMessage> roomInfoMessagesRepository, IRepository<Room> roomsRepository, IUserService userService, IUserContext userContext)
    {
        this.roomInfoMessagesRepository = roomInfoMessagesRepository;
        this.roomsRepository = roomsRepository;
        this.userService = userService;
        this.userContext = userContext;
    }

    public async Task<List<GetRoomInfoMessageListQueryModel>> Handle(GetRoomInfoMessageListQuery request, CancellationToken cancellationToken)
    {
        var room = await this.roomsRepository.GetByAsync(g => g.Id == request.Id, CancellationToken.None)
            ?? throw new NotFoundException(nameof(Room), request.Id);

        var messages = await this.roomInfoMessagesRepository
            .GetWithPropertiesAsync(x => x.RoomId == room.Id,
            m => new GetRoomInfoMessageListQueryModel
            {
                Id = m.Id,
                Message = m.MessageContent,
                CreatedDate = m.CreatedDate,
            }, CancellationToken.None);

        return messages;
    }
}
