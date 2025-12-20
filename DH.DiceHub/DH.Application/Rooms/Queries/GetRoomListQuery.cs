using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Models.RoomModels.Queries;
using DH.Domain.Services;
using MediatR;

namespace DH.Application.Rooms.Queries;

public record GetRoomListQuery(string? SearchExpression) : IRequest<List<GetRoomListQueryModel>>;

internal class GetRoomListQueryHandler : IRequestHandler<GetRoomListQuery, List<GetRoomListQueryModel>>
{
    readonly IRoomService roomService;
    readonly IUserManagementService userManagementService;

    public GetRoomListQueryHandler(
        IRoomService roomService, 
        IUserManagementService userManagementService)
    {
        this.roomService = roomService;
        this.userManagementService = userManagementService;
    }

    public async Task<List<GetRoomListQueryModel>> Handle(GetRoomListQuery request, CancellationToken cancellationToken)
    {
        var rooms = await this.roomService.GetListBySearchExpressionAsync(request.SearchExpression ?? string.Empty, cancellationToken);

        var userIds = rooms.Select(x => x.UserId).Distinct().ToArray();
        var users = await this.userManagementService.GetUserListByIds(userIds, cancellationToken);

        foreach (var room in rooms)
        {
            var user = users.FirstOrDefault(x => x.Id == room.UserId);
            if (user != null)
                room.Username = user.UserName;
        }

        return rooms;
    }
}
