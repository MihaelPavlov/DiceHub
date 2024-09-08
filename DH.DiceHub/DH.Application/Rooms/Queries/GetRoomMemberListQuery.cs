using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Models.RoomModels.Queries;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Rooms.Queries;

public record GetRoomMemberListQuery(int Id, string? SearchExpression) : IRequest<List<GetRoomMemberListQueryModel>>;

internal class GetRoomMemberListQueryHandler : IRequestHandler<GetRoomMemberListQuery, List<GetRoomMemberListQueryModel>>
{
    readonly IRepository<RoomParticipant> roomParticipantRepository;
    readonly IRepository<Room> roomsRepository;
    readonly IUserService userService;

    public GetRoomMemberListQueryHandler(IRepository<RoomParticipant> roomParticipantRepository, IRepository<Room> roomsRepository, IUserService userService)
    {
        this.roomParticipantRepository = roomParticipantRepository;
        this.roomsRepository = roomsRepository;
        this.userService = userService;
    }

    public async Task<List<GetRoomMemberListQueryModel>> Handle(GetRoomMemberListQuery request, CancellationToken cancellationToken)
    {
        var room = await this.roomsRepository.GetByAsync(g => g.Id == request.Id, CancellationToken.None)
            ?? throw new NotFoundException(nameof(Room), request.Id);

        var members = await this.roomParticipantRepository.GetWithPropertiesAsync(
            x => x.RoomId == room.Id && x.IsDeleted == false,
            x => new GetRoomMemberListQueryModel
            {
                UserId = x.UserId,
                JoinedAt = DateTime.Now,
                //TODO: Update this to get the age from the user service
                Age = 18,
            },
            CancellationToken.None);

        members.Insert(0, new GetRoomMemberListQueryModel
        {
            UserId = room.UserId,
            JoinedAt = room.CreatedDate,
            //TODO: Update this to get the age from the user service
            Age = 18
        });

        var userIds = members.Select(x => x.UserId).Distinct().ToArray();

        var users = await this.userService.GetUserListByIds(userIds, cancellationToken);

        foreach (var member in members)
        {
            var user = users.FirstOrDefault(x => x.Id == member.UserId);
            if (user != null)
                member.Username = user.UserName;
        }

        return string.IsNullOrEmpty(request.SearchExpression)
            ? members
            : members
                .Where(x => x.Username
                    .ToLower()
                    .Contains(request.SearchExpression
                    .ToLower()))
                .ToList();
    }
}
