using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.Localization;
using DH.Domain.Entities;
using DH.Domain.Models.SpaceManagementModels.Queries;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.SpaceManagement.Queries;

public record GetSpaceTableParticipantListQuery(int Id, string SearchExpression) : IRequest<List<GetSpaceTableParticipantListQueryModel>>;

internal class GetSpaceTableParticipantListQueryHandler : IRequestHandler<GetSpaceTableParticipantListQuery, List<GetSpaceTableParticipantListQueryModel>>
{
    readonly IRepository<SpaceTableParticipant> repository;
    readonly IUserManagementService userManagementService;
    readonly ILocalizationService localizer;

    public GetSpaceTableParticipantListQueryHandler(
        IRepository<SpaceTableParticipant> repository, 
        IUserManagementService userManagementService,
        ILocalizationService localizer)
    {
        this.repository = repository;
        this.userManagementService = userManagementService;
        this.localizer = localizer;
    }

    public async Task<List<GetSpaceTableParticipantListQueryModel>> Handle(GetSpaceTableParticipantListQuery request, CancellationToken cancellationToken)
    {
        var participants = await this.repository.GetWithPropertiesAsync(
            x => x.SpaceTableId == request.Id && x.SpaceTable.IsTableActive,
            x => new GetSpaceTableParticipantListQueryModel
            {
                ParticipantId = x.Id,
                UserId = x.UserId,
                JoinedBefore = ((int)(DateTime.UtcNow - x.JoinedAt).TotalMinutes),
                SpaceTableId = request.Id,
                IsVirtualParticipant = x.IsVirtualParticipant,

            }, cancellationToken);

        var userIds = participants.Where(x => !x.IsVirtualParticipant).Select(x => x.UserId).ToArray();

        var users = await this.userManagementService.GetUserListByIds(userIds, CancellationToken.None);

        var virtualUserCount = 1;
        foreach (var participant in participants)
        {
            if (participant.IsVirtualParticipant)
            {
                participant.UserName = string.Format(this.localizer["RoomVirtualUserUsername"], virtualUserCount);
                virtualUserCount++;
            }
            else
            {
                participant.UserName = users.First(x => x.Id == participant.UserId).UserName;
            }
        }

        return participants.Where(x => x.UserName.ToLower().Contains(request.SearchExpression.ToLower())).ToList();
    }
}