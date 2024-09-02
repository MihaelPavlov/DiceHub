using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Models.RoomModels.Commands;
using DH.Domain.Repositories;
using Mapster;
using MediatR;

namespace DH.Application.Rooms.Commands;

public record CreateRoomCommand(CreateRoomCommandDto Room) : IRequest<int>;

internal class CreateRoomCommandHanler : IRequestHandler<CreateRoomCommand, int>
{
    readonly IRepository<Room> roomRepository;
    readonly IRepository<Game> gameRepository;

    public CreateRoomCommandHanler(IRepository<Room> roomRepository, IRepository<Game> gameRepository)
    {
        this.roomRepository = roomRepository;
        this.gameRepository = gameRepository;
    }

    public async Task<int> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
    {
        if (!request.Room.FieldsAreValid(out var validationErrors))
            throw new ValidationErrorsException(validationErrors);

        var game = await gameRepository.GetByAsync(x => x.Id == request.Room.GameId, cancellationToken)
            ?? throw new NotFoundException(nameof(Game), request.Room.GameId);

        var room = await roomRepository.AddAsync(request.Room.Adapt<Room>(), cancellationToken);

        return room.Id;
    }
}
