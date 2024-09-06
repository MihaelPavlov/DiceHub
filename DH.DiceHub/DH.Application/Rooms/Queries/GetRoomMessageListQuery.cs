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
    readonly IRepository<Room> roomsRepository;

    public GetRoomMessageListQueryHandler(IRepository<RoomMessages> roomMessagesRepository, IRepository<Room> roomsRepository)
    {
        this.roomMessagesRepository = roomMessagesRepository;
        this.roomsRepository = roomsRepository;
    }

    public async Task<List<GetRoomMessageListQueryModel>> Handle(GetRoomMessageListQuery request, CancellationToken cancellationToken)
    {
        var room = await this.roomsRepository.GetByAsync(g => g.Id == request.Id, CancellationToken.None)
            ?? throw new NotFoundException(nameof(Room), request.Id);

        var messages = await this.roomMessagesRepository
            .GetWithPropertiesAsync(x => x.RoomId == room.Id,
            m => new GetRoomMessageListQueryModel
            {
                Id = m.Id,
                Message = m.MessageContent,
                CreatedDate = m.Timestamp,
                SenderId = m.Sender,
            }, CancellationToken.None);

        return messages;
    }
}
