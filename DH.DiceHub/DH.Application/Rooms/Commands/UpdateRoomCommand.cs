using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Models.RoomModels.Commands;
using DH.Domain.Repositories;
using Mapster;
using MediatR;

namespace DH.Application.Rooms.Commands;

public record UpdateRoomCommand(UpdateRoomCommandDto Room) : IRequest;

internal class UpdateRoomCommandHanler : IRequestHandler<UpdateRoomCommand>
{
    readonly IRepository<Room> roomRepository;
    readonly IRepository<Game> gameRepository;
    readonly IUserContext userContext;

    public UpdateRoomCommandHanler(IRepository<Room> roomRepository, IRepository<Game> gameRepository, IUserContext userContext)
    {
        this.roomRepository = roomRepository;
        this.gameRepository = gameRepository;
        this.userContext = userContext;
    }

    public async Task Handle(UpdateRoomCommand request, CancellationToken cancellationToken)
    {
        if (!request.Room.FieldsAreValid(out var validationErrors))
            throw new ValidationErrorsException(validationErrors);

        var game = await this.gameRepository.GetByAsync(x => x.Id == request.Room.GameId, cancellationToken)
            ?? throw new NotFoundException(nameof(Game), request.Room.GameId);

        var room = await this.roomRepository.GetByAsync(x => x.Id == request.Room.Id, cancellationToken)
           ?? throw new NotFoundException(nameof(Room), request.Room.Id);

        room.StartDate = request.Room.StartDate.AddHours(3);
        room.GameId = request.Room.GameId;
        room.Name = request.Room.Name;
        room.MaxParticipants = request.Room.MaxParticipants;

        await this.roomRepository.Update(room, cancellationToken);
    }
}
