using DH.Domain.Adapters.Authentication;
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
            throw new BadRequestException("Maximum number of participants reached.");

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
    }
}