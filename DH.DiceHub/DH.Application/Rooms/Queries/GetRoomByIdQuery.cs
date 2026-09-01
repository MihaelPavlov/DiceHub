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
    readonly IUserManagementService userManagementService;

    public GetRoomByIdQueryHandler(
        IRoomService roomService, IUserManagementService userManagementService)
    {
        this.roomService = roomService;
        this.userManagementService = userManagementService;
    }

    public async Task<GetRoomByIdQueryModel> Handle(GetRoomByIdQuery request, CancellationToken cancellationToken)
    {
        var room = await this.roomService.GetById(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Room), request.Id);

        var users = await this.userManagementService.GetUserListByIds([room.CreatedBy], cancellationToken);

        var user = users.FirstOrDefault(x => x.Id == room.CreatedBy);
        if (user != null)
            room.Username = user.UserName;

        return room;
    }
}