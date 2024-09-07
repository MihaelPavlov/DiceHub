using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Rooms.Commands;

public record JoinRoomCommand(int Id) : IRequest;

internal class JoinRoomCommandHandler : IRequestHandler<JoinRoomCommand>
{
    readonly IRepository<Room> roomsRepository;
    readonly IRepository<RoomParticipant> roomParticipantsRepository;
    readonly IUserContext userContext;

    public JoinRoomCommandHandler(IRepository<Room> roomsRepository, IRepository<RoomParticipant> roomParticipantsRepository, IUserContext userContext)
    {
        this.roomsRepository = roomsRepository;
        this.roomParticipantsRepository = roomParticipantsRepository;
        this.userContext = userContext;
    }

    public async Task Handle(JoinRoomCommand request, CancellationToken cancellationToken)
    {
        var room = await this.roomsRepository.GetByAsync(g => g.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Room), request.Id);

        var roomParticipantList = await this.roomParticipantsRepository.GetWithPropertiesAsync(x => x.RoomId == room.Id, x=> new {Id = x.Id }, cancellationToken);
        if (room.MaxParticipants == roomParticipantList.Count)
            throw new BadRequestException("Maximum number of participants reached.");

        var roomParticipant = new RoomParticipant { UserId = this.userContext.UserId, Room = room };
        await this.roomParticipantsRepository.AddAsync(roomParticipant, cancellationToken);
    }
}