using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Models.RoomModels.Queries;
using DH.Domain.Services;
using MediatR;

namespace DH.Application.Rooms.Queries;

public record GetRoomListQuery(string? SearchExpression) : IRequest<List<GetRoomListQueryModel>>;

internal class GetRoomListQueryHandler : IRequestHandler<GetRoomListQuery, List<GetRoomListQueryModel>>
{
    readonly IRoomService roomService;
    readonly IUserService userService;

    public GetRoomListQueryHandler(IRoomService roomService, IUserService userService)
    {
        this.roomService = roomService;
        this.userService = userService;
    }

    public async Task<List<GetRoomListQueryModel>> Handle(GetRoomListQuery request, CancellationToken cancellationToken)
    {
        var rooms = await this.roomService.GetListBySearchExpressionAsync(request.SearchExpression ?? string.Empty, cancellationToken);

        var userIds = rooms.Select(x => x.UserId).Distinct().ToArray();
        var users = await this.userService.GetUserListByIds(userIds, cancellationToken);

        foreach (var room in rooms)
        {
            var user = users.FirstOrDefault(x => x.Id == room.UserId);
            if (user != null)
                room.Username = user.UserName;
        }

        return rooms;
    }
}
