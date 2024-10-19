using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Entities;
using DH.Domain.Models.SpaceManagementModels.Queries;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.SpaceManagement.Queries;

public record GetSpaceTableParticipantListQuery(int Id, string SearchExpression) : IRequest<List<GetSpaceTableParticipantListQueryModel>>;

internal class GetSpaceTableParticipantListQueryHandler : IRequestHandler<GetSpaceTableParticipantListQuery, List<GetSpaceTableParticipantListQueryModel>>
{
    readonly IRepository<SpaceTableParticipant> repository;
    readonly IUserService userService;

    public GetSpaceTableParticipantListQueryHandler(IRepository<SpaceTableParticipant> repository, IUserService userService)
    {
        this.repository = repository;
        this.userService = userService;
    }

    public async Task<List<GetSpaceTableParticipantListQueryModel>> Handle(GetSpaceTableParticipantListQuery request, CancellationToken cancellationToken)
    {
        var participants = await this.repository.GetWithPropertiesAsync(
            x => x.SpaceTableId == request.Id && x.SpaceTable.IsTableActive,
            x => new GetSpaceTableParticipantListQueryModel
            {
                UserId = x.UserId,
                JoinedAt = x.JoinedAt,
                SpaceTableId = request.Id,
            }, cancellationToken);

        var userIds = participants.Select(x => x.UserId).ToArray();

        var users = await this.userService.GetUserListByIds(userIds, CancellationToken.None);

        foreach (var participant in participants)
        {
            participant.UserName = users.First(x => x.Id == participant.UserId).UserName;
        }

        return participants.Where(x => x.UserName.ToLower().Contains(request.SearchExpression.ToLower())).ToList();
    }
}