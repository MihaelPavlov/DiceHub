using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using DH.Domain.Models.RoomModels.Commands;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using Mapster;
using MediatR;

namespace DH.Application.Rooms.Commands;

public record CreateRoomCommand(CreateRoomCommandDto Room) : IRequest<int>;

internal class CreateRoomCommandHanler : IRequestHandler<CreateRoomCommand, int>
{
    readonly IRepository<Room> roomRepository;
    readonly IRepository<Game> gameRepository;
    readonly IUserContext userContext;

    public CreateRoomCommandHanler(IRepository<Room> roomRepository, IRepository<Game> gameRepository, IUserContext userContext)
    {
        this.roomRepository = roomRepository;
        this.gameRepository = gameRepository;
        this.userContext = userContext;
    }

    public async Task<int> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
    {
        if (!request.Room.FieldsAreValid(out var validationErrors))
            throw new ValidationErrorsException(validationErrors);

        var game = await this.gameRepository.GetByAsync(x => x.Id == request.Room.GameId, cancellationToken)
            ?? throw new NotFoundException(nameof(Game), request.Room.GameId);

        var roomDto = request.Room.Adapt<Room>();

        roomDto.StartDate = roomDto.StartDate.AddHours(3);
        roomDto.CreatedDate = DateTime.UtcNow;
        roomDto.UserId = this.userContext.UserId;

        var room = await this.roomRepository.AddAsync(roomDto, cancellationToken);
        return room.Id;
    }
}
