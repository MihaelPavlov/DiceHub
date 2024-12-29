using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Entities;
using DH.Domain.Models.RoomModels.Queries;
using DH.Domain.Services;
using DH.OperationResultCore.Exceptions;
using MediatR;

namespace DH.Application.Rooms.Queries;

public record GetRoomByIdQuery(int Id) : IRequest<GetRoomByIdQueryModel>;

internal class GetRoomByIdQueryHandler : IRequestHandler<GetRoomByIdQuery, GetRoomByIdQueryModel>
{
    readonly IRoomService roomService;
    readonly IUserService userService;

    public GetRoomByIdQueryHandler(IRoomService roomService, IUserService userService)
    {
        this.roomService = roomService;
        this.userService = userService;
    }

    public async Task<GetRoomByIdQueryModel> Handle(GetRoomByIdQuery request, CancellationToken cancellationToken)
    {
        var room = await this.roomService.GetById(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Room), request.Id);

        var users = await this.userService.GetUserListByIds([room.CreatedBy], cancellationToken);

        var user = users.FirstOrDefault(x => x.Id == room.CreatedBy);
        if (user != null)
            room.Username = user.UserName;

        return room;
    }
}