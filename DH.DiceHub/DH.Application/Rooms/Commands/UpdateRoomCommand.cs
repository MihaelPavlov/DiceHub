using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using DH.Domain.Models.RoomModels.Commands;
using DH.Domain.Repositories;
using DH.Domain.Services;
using DH.OperationResultCore.Exceptions;
using Mapster;
using MediatR;

namespace DH.Application.Rooms.Commands;

public record UpdateRoomCommand(UpdateRoomCommandDto Room) : IRequest;

internal class UpdateRoomCommandHanler : IRequestHandler<UpdateRoomCommand>
{
    readonly IRepository<Room> roomRepository;
    readonly IRepository<RoomInfoMessage> roomInfoMessagesRepository;
    readonly IRepository<Game> gameRepository;
    readonly IUserContext userContext;
    readonly IRoomService roomService;

    public UpdateRoomCommandHanler(IRepository<Room> roomRepository, IRepository<RoomInfoMessage> roomInfoMessagesRepository, IRepository<Game> gameRepository, IUserContext userContext, IRoomService roomService)
    {
        this.roomRepository = roomRepository;
        this.roomInfoMessagesRepository = roomInfoMessagesRepository;
        this.gameRepository = gameRepository;
        this.userContext = userContext;
        this.roomService = roomService;
    }

    public async Task Handle(UpdateRoomCommand request, CancellationToken cancellationToken)
    {
        if (!request.Room.FieldsAreValid(out var validationErrors))
            throw new ValidationErrorsException(validationErrors);

        await this.roomService.Update(request.Room.Adapt<Room>(), cancellationToken);       
    }
}
