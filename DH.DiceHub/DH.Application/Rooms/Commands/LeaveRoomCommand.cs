using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Rooms.Commands;

public record LeaveRoomCommand(int Id) : IRequest;

internal class LeaveRoomCommandHandler : IRequestHandler<LeaveRoomCommand>
{
    readonly IRepository<Room> roomsRepository;
    readonly IRepository<RoomParticipant> roomParticipantsRepository;
    readonly IUserContext userContext;

    public LeaveRoomCommandHandler(IRepository<Room> roomsRepository, IRepository<RoomParticipant> roomParticipantsRepository, IUserContext userContext)
    {
        this.roomsRepository = roomsRepository;
        this.roomParticipantsRepository = roomParticipantsRepository;
        this.userContext = userContext;
    }

    public async Task Handle(LeaveRoomCommand request, CancellationToken cancellationToken)
    {
        var room = await this.roomsRepository.GetByAsync(g => g.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Room), request.Id);

        var roomParticipant = await this.roomParticipantsRepository.GetByAsync(g => g.RoomId == request.Id && g.UserId == this.userContext.UserId, cancellationToken)
          ?? throw new NotFoundException(nameof(RoomParticipant));

        roomParticipant.IsDeleted = true;
        await this.roomParticipantsRepository.Update(roomParticipant, cancellationToken);
    }
}
