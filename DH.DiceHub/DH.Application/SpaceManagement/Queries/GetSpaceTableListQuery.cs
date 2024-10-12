using DH.Domain.Entities;
using DH.Domain.Models.SpaceManagementModels.Queries;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.SpaceManagement.Queries;

public record GetSpaceTableListQuery(string searchExpressionName) : IRequest<List<GetSpaceTableListQueryModel>>;

internal class GetSpaceTableListQueryHandler : IRequestHandler<GetSpaceTableListQuery, List<GetSpaceTableListQueryModel>>
{
    readonly IRepository<SpaceTable> repository;

    public GetSpaceTableListQueryHandler(IRepository<SpaceTable> repository)
    {
        this.repository = repository;
    }

    public async Task<List<GetSpaceTableListQueryModel>> Handle(GetSpaceTableListQuery request, CancellationToken cancellationToken)
    {
        return await this.repository.GetWithPropertiesAsync(
            x => x.Name.ToLower().Contains(request.searchExpressionName.ToLower()),
            x => new GetSpaceTableListQueryModel
            {
                GameImageId = x.Game.Image.Id,
                GameName = x.Game.Name,
                IsLocked = !string.IsNullOrEmpty(x.Password),
                MaxPeople = x.MaxPeople,
                PeopleJoined = x.SpaceTableParticipants.Count,
                TableName = x.Name,
                CreatedBy=x.CreatedBy,
            }, cancellationToken);
    }
}