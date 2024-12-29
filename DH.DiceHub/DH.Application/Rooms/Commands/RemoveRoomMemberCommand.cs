using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;

namespace DH.Application.Rooms.Commands;

public record RemoveRoomMemberCommand(int Id, string UserId) : IRequest;

internal class RemoveRoomMemberCommandHandler : IRequestHandler<RemoveRoomMemberCommand>
{
    readonly IRepository<Room> roomsRepository;
    readonly IRepository<RoomParticipant> roomParticipantsRepository;
    readonly IUserContext userContext;

    public RemoveRoomMemberCommandHandler(IRepository<Room> roomsRepository, IRepository<RoomParticipant> roomParticipantsRepository, IUserContext userContext)
    {
        this.roomsRepository = roomsRepository;
        this.roomParticipantsRepository = roomParticipantsRepository;
        this.userContext = userContext;
    }

    public async Task Handle(RemoveRoomMemberCommand request, CancellationToken cancellationToken)
    {
        var room = await this.roomsRepository.GetByAsync(g => g.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Room), request.Id);

        var roomParticipantList = await this.roomParticipantsRepository
            .GetWithPropertiesAsync(
                x => x.RoomId == room.Id,
                x => new RoomParticipant
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    RoomId = x.RoomId,
                    IsDeleted = x.IsDeleted,
                    JoinedAt = x.JoinedAt,
                },
                cancellationToken);

        var participantForDeletion = roomParticipantList.FirstOrDefault(x => x.UserId == request.UserId);
        if (participantForDeletion is null)
            throw new BadRequestException("Participant not found");

        participantForDeletion.IsDeleted = true;

        await this.roomParticipantsRepository.Update(participantForDeletion, cancellationToken);
    }
}
