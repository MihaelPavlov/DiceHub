using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Rooms.Queries;

public record GetUserRoomParticipationStatusQuery(int Id) : IRequest<bool>;

internal class GetUserRoomParticipationStatusQueryHandler : IRequestHandler<GetUserRoomParticipationStatusQuery, bool>
{
    readonly IRepository<Room> roomsRepository;
    readonly IRepository<RoomParticipant> roomParticipantsRepository;
    readonly IUserContext userContext;

    public GetUserRoomParticipationStatusQueryHandler(IRepository<Room> roomsRepository, IRepository<RoomParticipant> roomParticipantsRepository, IUserContext userContext)
    {
        this.roomsRepository = roomsRepository;
        this.roomParticipantsRepository = roomParticipantsRepository;
        this.userContext = userContext;
    }

    public async Task<bool> Handle(GetUserRoomParticipationStatusQuery request, CancellationToken cancellationToken)
    {
        var room = await this.roomsRepository.GetByAsync(g => g.Id == request.Id, CancellationToken.None)
             ?? throw new NotFoundException(nameof(Room), request.Id);

        var roomParticipant = await this.roomParticipantsRepository
            .GetByAsync(
                g => g.UserId == this.userContext.UserId &&
                g.RoomId == room.Id && 
                !g.IsDeleted,
                CancellationToken.None);

        if (roomParticipant is null)
            return false;

        return true;
    }
}

